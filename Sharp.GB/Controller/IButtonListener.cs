namespace Sharp.GB.Controller;

public interface IButtonListener
{
    void OnButtonPress(Button button);

    void OnButtonRelease(Button button);
}

public class Button
{
    public static Button Right => new(0x01, 0x10);
    public static Button Left => new(0x02, 0x10);
    public static Button Up => new(0x04, 0x10);
    public static Button Down => new(0x08, 0x10);
    public static Button A => new(0x01, 0x20);
    public static Button B => new(0x02, 0x20);
    public static Button Select => new(0x04, 0x20);
    public static Button Start => new(0x08, 0x20);

    private readonly int _mask;

    private readonly int _line;

    Button(int mask, int line)
    {
        _mask = mask;
        _line = line;
    }

    public int GetMask()
    {
        return _mask;
    }

    public int GetLine()
    {
        return _line;
    }
}
