namespace Sharp.GB.Sound;

public class LengthCounter
{
    private readonly int _divider = Gameboy.TicksPerSec / 256;

    private readonly int _fullLength;

    private int _length;

    private long _i;

    private bool _enabled;

    public LengthCounter(int fullLength)
    {
        this._fullLength = fullLength;
    }

    public void Start()
    {
        _i = 8192;
    }

    public void Tick()
    {
        if (++_i == _divider)
        {
            _i = 0;
            if (_enabled && _length > 0)
            {
                _length--;
            }
        }
    }

    public void SetLength(int length)
    {
        if (length == 0)
        {
            this._length = _fullLength;
        }
        else
        {
            this._length = length;
        }
    }

    public void SetNr4(int value)
    {
        bool enable = (value & (1 << 6)) != 0;
        bool trigger = (value & (1 << 7)) != 0;

        if (_enabled)
        {
            if (_length == 0 && trigger)
            {
                if (enable && _i < _divider / 2)
                {
                    SetLength(_fullLength - 1);
                }
                else
                {
                    SetLength(_fullLength);
                }
            }
        }
        else if (enable)
        {
            if (_length > 0 && _i < _divider / 2)
            {
                _length--;
            }

            if (_length == 0 && trigger && _i < _divider / 2)
            {
                SetLength(_fullLength - 1);
            }
        }
        else
        {
            if (_length == 0 && trigger)
            {
                SetLength(_fullLength);
            }
        }

        _enabled = enable;
    }

    public int GetValue()
    {
        return _length;
    }

    public bool IsEnabled()
    {
        return _enabled;
    }

    public override string ToString()
    {
        return string.Format(
            "LengthCounter[l=%d,f=%d,c=%d,%s]",
            _length,
            _fullLength,
            _i,
            _enabled ? "enabled" : "disabled"
        );
    }

    public void Reset()
    {
        _enabled = true;
        _i = 0;
        _length = 0;
    }
}
