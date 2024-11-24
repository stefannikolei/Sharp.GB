using Sharp.GB.Cpu.Op;
using Sharp.GB.Cpu.OpCode;
using Sharp.GB.Gpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu;

public class Cpu
{
    public enum State
    {
        Opcode,
        ExtOpcode,
        Operand,
        Running,
        IrqReadIf,
        IrqReadIe,
        IrqPush1,
        IrqPush2,
        IrqJump,
        Stopped,
        Halted,
    }

    private readonly Registers _registers;

    private readonly IAddressSpace _addressSpace;

    private readonly InterruptManager _interruptManager;

    private readonly Gpu.Gpu? _gpu;

    private readonly IDisplay _display;

    private readonly SpeedMode _speedMode;

    private int _opcode1,
        _opcode2;

    private int[] _operand = new int[2];

    private Opcode? _currentOpcode;

    private List<IOp>? _ops;

    private int _operandIndex;

    private int _opIndex;

    private State _state = State.Opcode;

    private int _opContext;

    private int _interruptFlag;

    private int _interruptEnabled;

    private InterruptType? _requestedIrq;

    private int _clockCycle = 0;

    private bool _haltBugMode;

    public Cpu(
        IAddressSpace addressSpace,
        InterruptManager interruptManager,
        Gpu.Gpu? gpu,
        IDisplay display,
        SpeedMode speedMode
    )
    {
        _registers = new();
        _addressSpace = addressSpace;
        _interruptManager = interruptManager;
        _gpu = gpu;
        _display = display;
        _speedMode = speedMode;
    }

    public void Tick()
    {
        if (++_clockCycle >= (4 / _speedMode.GetSpeedMode()))
        {
            _clockCycle = 0;
        }
        else
        {
            return;
        }

        if (_state == State.Opcode || _state == State.Halted || _state == State.Stopped)
        {
            if (_interruptManager.IsIme() && _interruptManager.IsInterruptRequested())
            {
                if (_state == State.Stopped)
                {
                    _display.EnableLcd();
                }
                _state = State.IrqReadIf;
            }
        }

        if (
            _state == State.IrqReadIf
            || _state == State.IrqReadIe
            || _state == State.IrqPush1
            || _state == State.IrqPush2
            || _state == State.IrqJump
        )
        {
            HandleInterrupt();
            return;
        }

        if (_state == State.Halted && _interruptManager.IsInterruptRequested())
        {
            _state = State.Opcode;
        }

        if (_state is State.Halted or State.Stopped)
        {
            return;
        }

        bool accessedMemory = false;
        while (true)
        {
            int pc = _registers.GetPc();
            switch (_state)
            {
                case State.Opcode:
                    ClearState();
                    _opcode1 = _addressSpace.GetByte(pc);
                    accessedMemory = true;
                    if (_opcode1 == 0xcb)
                    {
                        _state = State.ExtOpcode;
                    }
                    else if (_opcode1 == 0x10)
                    {
                        _currentOpcode = OpCodes.Commands[_opcode1];
                        _state = State.ExtOpcode;
                    }
                    else
                    {
                        _state = State.Operand;
                        _currentOpcode = OpCodes.Commands[_opcode1];
                        if (_currentOpcode == null)
                        {
                            throw new ArgumentException(
                                string.Format("No command for 0x%02x", _opcode1)
                            );
                        }
                    }
                    if (!_haltBugMode)
                    {
                        _registers.IncrementPc();
                    }
                    else
                    {
                        _haltBugMode = false;
                    }
                    break;

                case State.ExtOpcode:
                    if (accessedMemory)
                    {
                        return;
                    }
                    accessedMemory = true;
                    _opcode2 = _addressSpace.GetByte(pc);
                    if (_currentOpcode == null)
                    {
                        _currentOpcode = OpCodes.Commands[_opcode2];
                    }
                    if (_currentOpcode == null)
                    {
                        throw new ApplicationException(
                            string.Format("No command for %0xcb 0x%02x", _opcode2)
                        );
                    }
                    _state = State.Operand;
                    _registers.IncrementPc();
                    break;

                case State.Operand:
                    ArgumentNullException.ThrowIfNull(_currentOpcode);
                    while (_operandIndex < _currentOpcode.GetOperandLength())
                    {
                        if (accessedMemory)
                        {
                            return;
                        }
                        accessedMemory = true;
                        _operand[_operandIndex++] = _addressSpace.GetByte(pc);
                        _registers.IncrementPc();
                    }
                    _ops = _currentOpcode.GetOps();
                    _state = State.Running;
                    break;

                case State.Running:
                    ArgumentNullException.ThrowIfNull(_ops);
                    if (_opcode1 == 0x10)
                    {
                        if (_speedMode.OnStop())
                        {
                            _state = State.Opcode;
                        }
                        else
                        {
                            _state = State.Stopped;
                            _display.DisableLcd();
                        }
                        return;
                    }
                    else if (_opcode1 == 0x76)
                    {
                        if (_interruptManager.IsHaltBug())
                        {
                            _state = State.Opcode;
                            _haltBugMode = true;
                            return;
                        }
                        else
                        {
                            _state = State.Halted;
                            return;
                        }
                    }

                    if (_opIndex < _ops.Count)
                    {
                        IOp op = _ops[_opIndex];
                        bool opAccessesMemory = op.ReadsMemory() || op.WritesMemory();
                        if (accessedMemory && opAccessesMemory)
                        {
                            return;
                        }
                        _opIndex++;

                        SpriteBug.CorruptionType? corruptionType = op.CausesOemBug(
                            _registers,
                            _opContext
                        );
                        if (corruptionType != null)
                        {
                            HandleSpriteBug(corruptionType);
                        }
                        _opContext = op.Execute(_registers, _addressSpace, _operand, _opContext);
                        op.SwitchInterrupts(_interruptManager);

                        if (!op.Proceed(_registers))
                        {
                            _opIndex = _ops.Count;
                            break;
                        }

                        if (op.ForceFinishCycle())
                        {
                            return;
                        }

                        if (opAccessesMemory)
                        {
                            accessedMemory = true;
                        }
                    }

                    if (_opIndex >= _ops.Count)
                    {
                        _state = State.Opcode;
                        _operandIndex = 0;
                        _interruptManager.OnInstructionFinished();
                        return;
                    }
                    break;

                case State.Halted:
                case State.Stopped:
                    return;
            }
        }
    }

    private void HandleInterrupt()
    {
        switch (_state)
        {
            case State.IrqReadIf:
                _interruptFlag = _addressSpace.GetByte(0xff0f);
                _state = State.IrqReadIe;
                break;

            case State.IrqReadIe:
                _interruptEnabled = _addressSpace.GetByte(0xffff);
                _requestedIrq = null;
                foreach (InterruptType irq in InterruptType.All)
                {
                    if ((_interruptFlag & _interruptEnabled & (1 << irq.Ordinal())) != 0)
                    {
                        _requestedIrq = irq;
                        break;
                    }
                }
                if (_requestedIrq == null)
                {
                    _state = State.Opcode;
                }
                else
                {
                    _state = State.IrqPush1;
                    _interruptManager.ClearInterrupt(_requestedIrq);
                    _interruptManager.DisableInterrupts(false);
                }
                break;

            case State.IrqPush1:
                _registers.DecrementSp();
                _addressSpace.SetByte(_registers.GetSp(), (_registers.GetPc() & 0xff00) >> 8);
                _state = State.IrqPush2;
                break;

            case State.IrqPush2:
                _registers.DecrementSp();
                _addressSpace.SetByte(_registers.GetSp(), _registers.GetPc() & 0x00ff);
                _state = State.IrqJump;
                break;

            case State.IrqJump:
                ArgumentNullException.ThrowIfNull(_requestedIrq);
                _registers.SetPc(_requestedIrq.GetHandler());
                _requestedIrq = null;
                _state = State.Opcode;
                break;
        }
    }

    private void HandleSpriteBug(SpriteBug.CorruptionType? type)
    {
        if (_gpu is null || !_gpu.GetLcdc().IsLcdEnabled())
        {
            return;
        }
        int stat = _addressSpace.GetByte(GpuRegister.Stat.GetAddress());
        if ((stat & 0b11) == (int)Mode.OamSearch && _gpu.GetTicksInLine() < 79)
        {
            SpriteBug.CorruptOam(_addressSpace, type, _gpu.GetTicksInLine());
        }
    }

    public Registers GetRegisters()
    {
        return _registers;
    }

    public void ClearState()
    {
        _opcode1 = 0;
        _opcode2 = 0;
        _currentOpcode = null;
        _ops = null;

        _operand[0] = 0x00;
        _operand[1] = 0x00;
        _operandIndex = 0;

        _opIndex = 0;
        _opContext = 0;

        _interruptFlag = 0;
        _interruptEnabled = 0;
        _requestedIrq = null;
    }

    public State GetState()
    {
        return _state;
    }

    public Opcode? GetCurrentOpcode()
    {
        return _currentOpcode;
    }
}
