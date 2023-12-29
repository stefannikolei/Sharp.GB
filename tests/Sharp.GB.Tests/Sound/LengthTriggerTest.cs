using Xunit;

public class LengthTriggerTest : AbstractLengthCounterTest
{
    [Fact]
    public void Test02()
    {
        Begin();
        Wchn(1, -2);
        DelayClocks(8256);
        Wchn(4, 0x40);
        EndNoDelay(2);
    }

    [Fact]
    public void Test03()
    {
        Begin();
        Wchn(1, -2);
        DelayClocks(7900);
        Wchn(4, 0x40);
        EndNoDelay(1);
    }

    [Fact]
    public void Test04()
    {
        Begin();
        Wchn(4, 0x40);
        Wchn(1, -2);
        Wchn(4, 0x40);
        Wchn(4, 0x00);
        Wchn(4, 0x00);
        End(2);
    }

    [Fact]
    public void Test05()
    {
        Begin();
        Wchn(1, -1);
        Wchn(4, 0x40);
        Assert.True(_lengthCounter.IsEnabled());
        Assert.Equal(0, _lengthCounter.GetValue());
    }

    [Fact]
    public void Test06()
    {
        Begin();
        Wchn(1, -1);
        Wchn(4, 0x40);
        Wchn(4, 0);
        Wchn(4, 0x40);
        Wchn(4, 0);
        Wchn(4, 0x40);
        End(_maxlen);
    }

    [Fact]
    public void Test07()
    {
        Begin();
        Wchn(1, -1);
        Wchn(4, 0x40);
        Wchn(4, 0x00);
        Wchn(4, 0x80);
        DelayClocks(8192);
        Wchn(4, 0x40);
        DelayApu(2);
        EndNoDelay(_maxlen - 2);
    }

    [Fact]
    public void Test08()
    {
        Begin();
        Wchn(1, -1);
        Wchn(4, 0x40);
        Wchn(4, 0x00);
        Wchn(4, 0xc0);
        EndNoDelay(_maxlen - 1);

        Begin();
        Wchn(1, -1);
        Wchn(4, 0x40);
        Wchn(4, 0xc0);
        EndNoDelay(_maxlen - 1);
    }

    [Fact]
    public void Test09()
    {
        Begin();
        Wchn(1, -1);
        Wchn(4, 0xc0);
        EndNoDelay(_maxlen - 1);
    }

    [Fact]
    public void Test010()
    {
        Begin();
        Wchn(1, 0);
        DelayClocks(8192);
        Wchn(4, 0x80);
        EndNoDelay(_maxlen);
    }

    [Fact]
    public void Test012()
    {
        SyncApu();
        Wchn(4, 0);
        Wchn(1, -1);
        Wchn(4, 0x40);
        Wchn(4, 0);
        Wchn(4, 0x40);
        Wchn(4, 0x80);
        Wchn(4, 0x40);
        Wchn(4, 0);
        Wchn(4, 0x40);
        Wchn(4, 0xc0);
        DelayApu(_maxlen - 3);
        Assert.True(_lengthCounter.IsEnabled());
        Assert.NotEqual(0, _lengthCounter.GetValue());
        DelayApu(1);
        Assert.True(_lengthCounter.IsEnabled());
        Assert.Equal(0, _lengthCounter.GetValue());
    }

    private void Begin()
    {
        SyncApu();
        Wchn(1, -60);
        Wchn(4, 0x80);
    }

    private void End(int remainingLength)
    {
        DelayClocks(8192 + 1024);
        EndNoDelay(remainingLength);
    }

    private void EndNoDelay(int remainingLength)
    {
        Wchn(4, 0xc0);
        EndPassive(remainingLength);
    }

    private void EndPassive(int remainingLength)
    {
        Assert.Equal(remainingLength, _lengthCounter.GetValue());
    }
}
