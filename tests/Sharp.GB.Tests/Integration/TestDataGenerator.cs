using System.Reflection;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

public class TestDataGenerator
{
    public static IEnumerable<object[]> GetBlarggCgbSound()
    {
        return GetRoms("blargg/cgb_sound/");
    }

    public static IEnumerable<object[]> GetBlarggCpuInstrs()
    {
        return GetRoms("blargg/cpu_instrs/");
    }

    public static IEnumerable<object[]> GetBlarggDmgSound2()
    {
        return GetRoms("blargg/dmg_sound-2/");
    }

    public static IEnumerable<object[]> GetBlarggMemTiming()
    {
        return GetRoms("blargg/mem_timing-2/");
    }

    public static IEnumerable<object[]> GetBlarggOamBug()
    {
        return GetRoms("blargg/oam_bug-2/");
    }

    public static IEnumerable<object[]> GetMooneyAcceptanceBits()
    {
        return GetRoms("mooneye/acceptance/bits/");
    }

    public static IEnumerable<object[]> GetMooneyAcceptanceInstr()
    {
        return GetRoms("mooneye/acceptance/instr/");
    }

    public static IEnumerable<object[]> GetMooneyAcceptanceInterrupts()
    {
        return GetRoms("mooneye/acceptance/interrupts/");
    }

    public static IEnumerable<object[]> GetMooneyAcceptanceoam_dma()
    {
        return GetRoms("mooneye/acceptance/oam_dma/");
    }

    public static IEnumerable<object[]> GetMooneyAcceptancePpu()
    {
        return GetRoms("mooneye/acceptance/ppu/");
    }

    public static IEnumerable<object[]> GetMooneyAcceptanceSerial()
    {
        return GetRoms("mooneye/acceptance/serial/");
    }

    public static IEnumerable<object[]> GetMooneyAcceptanceTimer()
    {
        return GetRoms("mooneye/acceptance/timer/");
    }

    private static IEnumerable<object[]> GetRoms(string folder)
    {
        var path = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "resources",
            "roms",
            folder
        );

        var directoryInfo = new DirectoryInfo(path);

        foreach (var file in directoryInfo.EnumerateFiles())
        {
            yield return [file.FullName];
        }
    }
}
