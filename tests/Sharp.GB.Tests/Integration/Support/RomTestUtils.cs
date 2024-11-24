using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

public static class RomTestUtils
{
    public static void TestRomWithMemory(string romPath)
    {
        Console.WriteLine("\n### Running test rom " + Path.GetFileName(romPath) + " ###");
        MemoryTestRunner runner = new MemoryTestRunner(new FileInfo(romPath), Console.Out);
        MemoryTestRunner.TestResult result = runner.RunTest();
        Assert.Equal(0, result.Status);
    }

    public static void TestRomWithSerial(string romPath)
    {
        Console.WriteLine("\n### Running test rom " + Path.GetFileName(romPath) + " ###");
        SerialTestRunner runner = new SerialTestRunner(new FileInfo(romPath), Console.Out);
        string result = runner.RunTest();
        Assert.True(result.Contains("Passed"));
    }

    public static void TestMooneyeRom(string romPath, ITestOutputHelper output)
    {
        Console.WriteLine("\n### Running test rom " + Path.GetFileName(romPath) + " ###");
        MooneyeTestRunner runner = new MooneyeTestRunner(new FileInfo(romPath), output);
        Assert.True(runner.RunTest());
    }

    public static void TestRomWithImage(string romPath, ITestOutputHelper output)
    {
        Console.WriteLine("\n### Running test rom " + Path.GetFileName(romPath) + " ###");
        ImageTestRunner runner = new ImageTestRunner(new FileInfo(romPath), output);
        ImageTestRunner.TestResult result = runner.runTest();
    }
}
