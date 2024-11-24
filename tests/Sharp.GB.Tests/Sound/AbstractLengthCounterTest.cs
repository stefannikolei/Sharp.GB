using Sharp.GB.Sound;

public abstract class AbstractLengthCounterTest
{
    protected int _maxlen;
    protected LengthCounter _lengthCounter;

    public AbstractLengthCounterTest()
    {
        _maxlen = 256;
        _lengthCounter = new(_maxlen);
    }

    public AbstractLengthCounterTest(int maxlen)
    {
        _maxlen = maxlen;
        _lengthCounter = new(maxlen);
    }

    protected void Wchn(int register, int value)
    {
        if (register == 1)
        {
            _lengthCounter.SetLength(0 - value);
        }
        else if (register == 4)
        {
            _lengthCounter.SetNr4(value);
        }
        else
        {
            throw new ArgumentException();
        }
    }

    protected void DelayClocks(int clocks)
    {
        for (int i = 0; i < clocks; i++)
        {
            _lengthCounter.Tick();
        }
    }

    protected void DelayApu(int apuUnit)
    {
        DelayClocks(apuUnit * (Gameboy.TicksPerSec / 256));
    }

    protected void SyncApu()
    {
        _lengthCounter.Reset();
    }
}
