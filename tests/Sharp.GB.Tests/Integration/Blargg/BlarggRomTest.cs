using System.IO;
using System.Reflection;
using Xunit;
using static RomTestUtils;

public class BlarggRomTest
{
    [Fact(Skip = "true")]
    public void TestCgbSound()
    {
        TestRomWithMemory(GetPath("cgb_sound.gb"));
    }

    [Fact(Skip = "true")]
    public void TestCpuInstrs()
    {
        TestRomWithSerial(GetPath("cpu_instrs.gb"));
    }

    [Fact(Timeout = 100, Skip = "true")]
    public void TestDmgSound2()
    {
        TestRomWithMemory(GetPath("dmg_sound-2.gb"));
    }

    [Fact(Skip = "true")]
    public void TestHaltBug()
    {
        TestRomWithMemory(GetPath("halt_bug.gb"));
    }

    [Fact]
    public void TestInstrTiming()
    {
        TestRomWithSerial(GetPath("instr_timing.gb"));
    }

    [Fact(Skip = "true")]
    public void TestInterruptTime()
    {
        TestRomWithMemory(GetPath("interrupt_time.gb"));
    }

    [Fact(Skip = "true")]
    public void TestMemTiming2()
    {
        TestRomWithMemory(GetPath("mem_timing-2.gb"));
    }

    [Fact(Skip = "true-")]
    public void TestOamBug2()
    {
        TestRomWithMemory(GetPath("oam_bug-2.gb"));
    }

    private static string GetPath(string name)
    {
        return Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "resources",
            "roms",
            "blargg",
            name
        );
    }
}
