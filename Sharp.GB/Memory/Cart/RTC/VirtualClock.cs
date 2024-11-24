using System;

public class VirtualClock : IClock
{
    private long _clock = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public long CurrentTimeMillis()
    {
        return _clock;
    }

    public void Forward(long i, TimeSpan unit)
    {
        _clock += (long)unit.TotalMilliseconds * i;
    }
}
