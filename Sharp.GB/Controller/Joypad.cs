using Sharp.GB.Cpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Controller;

public class Joypad : IAddressSpace
{
    private HashSet<Button> _buttons = [];

    private int _p1;

    public Joypad(InterruptManager interruptManager, IController controller)
    {
        controller.SetButtonListener(new JoyPadButtonListener(interruptManager));
    }

    public bool Accepts(int address)
    {
        return address == 0xff00;
    }

    public void SetByte(int address, int value)
    {
        _p1 = value & 0b00110000;
    }

    public int GetByte(int address)
    {
        int result = _p1 | 0b11001111;
        foreach (Button b in _buttons)
        {
            if ((b.GetLine() & _p1) == 0)
            {
                result &= 0xff & ~b.GetMask();
            }
        }
        return result;
    }
}

public class JoyPadButtonListener : IButtonListener
{
    private readonly InterruptManager _interruptManager;
    private List<Button> _buttons = [];

    public JoyPadButtonListener(InterruptManager interruptManager)
    {
        _interruptManager = interruptManager;
    }

    public void OnButtonPress(Button button)
    {
        _interruptManager.RequestInterrupt(InterruptType.P1013);
        _buttons.Add(button);
    }

    public void OnButtonRelease(Button button)
    {
        _buttons.Remove(button);
    }
}
