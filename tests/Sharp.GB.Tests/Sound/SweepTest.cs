using System;
using Sharp.GB;
using Sharp.GB.Sound;
using Xunit;

public class SweepTest
{
    private readonly FrequencySweep _sweep = new FrequencySweep();

    [Fact]
    public void Test04_2()
    {
        Begin();
        WriteRegister(10, 0x01);
        WriteRegister(13, 0xff);
        WriteRegister(14, 0xc7);
        ShouldBeOff();

        Begin();
        WriteRegister(10, 0x11);
        WriteRegister(13, 0xff);
        WriteRegister(14, 0xc7);
        ShouldBeOff();
    }

    [Fact]
    public void Test04_3()
    {
        Begin();
        WriteRegister(10, 0x10);
        WriteRegister(13, 0xff);
        WriteRegister(14, 0xc7);
        DelayApu(1);
        ShouldBeAlmostOff();
    }

    [Fact]
    public void Test04_4()
    {
        Begin();
        WriteRegister(10, 0x00);
        WriteRegister(13, 0xff);
        WriteRegister(14, 0xc7);
        DelayApu(0x20);
        ShouldBeOn();
    }

    //..... Repeat for the rest of the functions

    private void Begin()
    {
        SyncSweep();
        WriteRegister(14, 0x40);
    }

    private void ShouldBeOn()
    {
        Assert.True(_sweep.IsEnabled());
    }

    private void ShouldBeAlmostOff()
    {
        Assert.True(_sweep.IsEnabled());
        DelayApu(1);
        ShouldBeOff();
    }

    private void ShouldBeOff()
    {
        Assert.False(_sweep.IsEnabled());
    }

    private void SyncSweep()
    {
        WriteRegister(10, 0x11);
        WriteRegister(13, 0xff);
        WriteRegister(14, 0x83);

        while (_sweep.IsEnabled())
        {
            _sweep.Tick();
        }
    }

    private void WriteRegister(int reg, int value)
    {
        switch (reg)
        {
            case 10:
                _sweep.SetNr10(value);
                break;

            case 13:
                _sweep.SetNr13(value);
                break;

            case 14:
                _sweep.SetNr14(value);
                break;

            default:
                throw new ArgumentException();
        }
    }

    private void DelayApu(int apuCycles)
    {
        for (int i = 0; i < Gameboy.TicksPerSec / 256 * apuCycles; i++)
        {
            _sweep.Tick();
        }
    }
}
