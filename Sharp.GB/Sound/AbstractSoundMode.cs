using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Sound;

public abstract class AbstractSoundMode : IAddressSpace
{
    protected readonly int Offset;
    protected readonly bool Gbc;

    protected bool _channelEnabled;
    protected bool _dacEnabled;
    protected int _nr0,
        _nr1,
        _nr2,
        _nr3,
        _nr4;
    protected LengthCounter _length;

    public AbstractSoundMode(int offset, int length, bool gbc)
    {
        Offset = offset;
        _length = new(length);
        Gbc = gbc;
    }

    public abstract int Tick();

    protected abstract void Trigger();

    public bool IsEnabled()
    {
        return _channelEnabled && _dacEnabled;
    }

    public bool Accepts(int address)
    {
        return address >= Offset && address < Offset + 5;
    }

    public void SetByte(int address, int value)
    {
        switch (address - Offset)
        {
            case 0:
                SetNr0(value);
                break;

            case 1:
                SetNr1(value);
                break;

            case 2:
                SetNr2(value);
                break;

            case 3:
                SetNr3(value);
                break;

            case 4:
                SetNr4(value);
                break;
        }
    }

    public int GetByte(int address)
    {
        switch (address - Offset)
        {
            case 0:
                return GetNr0();

            case 1:
                return GetNr1();

            case 2:
                return GetNr2();

            case 3:
                return GetNr3();

            case 4:
                return GetNr4();

            default:
                throw new ArgumentException(
                    "Illegal address for sound mode: " + address.ToString("X")
                );
        }
    }

    protected void SetNr0(int value)
    {
        _nr0 = value;
    }

    protected void SetNr1(int value)
    {
        _nr1 = value;
    }

    protected void SetNr2(int value)
    {
        _nr2 = value;
    }

    protected void SetNr3(int value)
    {
        _nr3 = value;
    }

    protected void SetNr4(int value)
    {
        _nr4 = value;
        _length.SetNr4(value);
        if ((value & (1 << 7)) != 0)
        {
            _channelEnabled = _dacEnabled;
            Trigger();
        }
    }

    protected int GetNr0()
    {
        return _nr0;
    }

    protected int GetNr1()
    {
        return _nr1;
    }

    protected int GetNr2()
    {
        return _nr2;
    }

    protected int GetNr3()
    {
        return _nr3;
    }

    protected int GetNr4()
    {
        return _nr4;
    }

    protected int GetFrequency()
    {
        return 2048 - (GetNr3() | ((GetNr4() & 0b111) << 8));
    }

    public abstract void Start();

    public void Stop()
    {
        _channelEnabled = false;
    }

    protected bool UpdateLength()
    {
        _length.Tick();
        if (!_length.IsEnabled())
        {
            return _channelEnabled;
        }

        if (_channelEnabled && _length.GetValue() == 0)
        {
            _channelEnabled = false;
        }

        return _channelEnabled;
    }
}
