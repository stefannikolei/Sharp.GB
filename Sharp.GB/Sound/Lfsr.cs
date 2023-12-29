namespace Sharp.GB.Sound;

public class Lfsr
{
    private int _lfsr;

    public Lfsr()
    {
        Reset();
    }

    public void Start()
    {
        Reset();
    }

    public void Reset()
    {
        _lfsr = 0x7fff;
    }

    public int NextBit(bool widthMode7)
    {
        bool x = ((_lfsr & 1) ^ ((_lfsr & 2) >> 1)) != 0;
        _lfsr = _lfsr >> 1;
        _lfsr = _lfsr | (x ? (1 << 14) : 0);
        if (widthMode7)
        {
            _lfsr = _lfsr | (x ? (1 << 6) : 0);
        }

        return 1 & ~_lfsr;
    }

    public int GetValue()
    {
        return _lfsr;
    }
}
