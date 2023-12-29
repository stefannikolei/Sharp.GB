using Sharp.GB.Sound;
using Xunit;

public class LfsrTest
{
    [Fact]
    public void TestLfsr()
    {
        Lfsr lfsr = new Lfsr();
        int previousValue = 0;
        for (int i = 0; i < 100; i++)
        {
            lfsr.NextBit(false);
            Assert.NotEqual(previousValue, lfsr.GetValue());
            previousValue = lfsr.GetValue();
        }
    }

    [Fact]
    public void TestLfsrWidth7()
    {
        Lfsr lfsr = new Lfsr();
        int previousValue = 0;
        for (int i = 0; i < 100; i++)
        {
            lfsr.NextBit(true);
            Assert.NotEqual(previousValue, lfsr.GetValue());
            previousValue = lfsr.GetValue();
        }
    }
}
