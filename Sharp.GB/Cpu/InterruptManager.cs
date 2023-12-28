using System.Collections.Generic;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu
{
    public class InterruptType
    {
        public static List<InterruptType> All => [VBlank, Lcdc, Timer, Serial, P1013];
        public static InterruptType VBlank => new InterruptType(0x0040, 0);
        public static InterruptType Lcdc => new InterruptType(0x0048, 1);
        public static InterruptType Timer => new InterruptType(0x0050, 2);
        public static InterruptType Serial => new InterruptType(0x0058, 3);
        public static InterruptType P1013 => new InterruptType(0x0060, 4);

        private readonly int _handler;
        private readonly int _number;

        private InterruptType(int handler, int number)
        {
            this._handler = handler;
            _number = number;
        }

        public int GetHandler()
        {
            return _handler;
        }

        public int Ordinal()
        {
            return _number;
        }
    }

    public class InterruptManager : IAddressSpace
    {
        private readonly bool _gbc;

        private bool _ime;

        private int _interruptFlag = 0xe1;

        private int _interruptEnabled;

        private int _pendingEnableInterrupts = -1;

        private int _pendingDisableInterrupts = -1;

        public InterruptManager(bool gbc)
        {
            this._gbc = gbc;
        }

        public void EnableInterrupts(bool withDelay)
        {
            _pendingDisableInterrupts = -1;
            if (withDelay)
            {
                if (_pendingEnableInterrupts == -1)
                {
                    _pendingEnableInterrupts = 1;
                }
            }
            else
            {
                _pendingEnableInterrupts = -1;
                _ime = true;
            }
        }

        public void DisableInterrupts(bool withDelay)
        {
            _pendingEnableInterrupts = -1;
            if (withDelay && _gbc)
            {
                if (_pendingDisableInterrupts == -1)
                {
                    _pendingDisableInterrupts = 1;
                }
            }
            else
            {
                _pendingDisableInterrupts = -1;
                _ime = false;
            }
        }

        public void RequestInterrupt(InterruptType type)
        {
            _interruptFlag = _interruptFlag | (1 << type.Ordinal());
        }

        public void ClearInterrupt(InterruptType type)
        {
            _interruptFlag = _interruptFlag & ~(1 << type.Ordinal());
        }

        public void OnInstructionFinished()
        {
            if (_pendingEnableInterrupts != -1)
            {
                if (_pendingEnableInterrupts-- == 0)
                {
                    EnableInterrupts(false);
                }
            }

            if (_pendingDisableInterrupts != -1)
            {
                if (_pendingDisableInterrupts-- == 0)
                {
                    DisableInterrupts(false);
                }
            }
        }

        public bool IsIme()
        {
            return _ime;
        }

        public bool IsInterruptRequested()
        {
            return (_interruptFlag & _interruptEnabled) != 0;
        }

        public bool IsHaltBug()
        {
            return (_interruptFlag & _interruptEnabled & 0x1f) != 0 && !_ime;
        }

        public bool Accepts(int address)
        {
            return address == 0xff0f || address == 0xffff;
        }

        public void SetByte(int address, int value)
        {
            switch (address)
            {
                case 0xff0f:
                    _interruptFlag = value | 0xe0;
                    break;

                case 0xffff:
                    _interruptEnabled = value;
                    break;
            }
        }

        public int GetByte(int address)
        {
            switch (address)
            {
                case 0xff0f:
                    return _interruptFlag;

                case 0xffff:
                    return _interruptEnabled;

                default:
                    return 0xff;
            }
        }
    }
}
