using System;

public interface IClock
{
    long CurrentTimeMillis();
}

public class SystemClock : IClock
{
    public long CurrentTimeMillis()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
