using Xunit;

public class BlarggTests
{
    [
        Theory,
        MemberData(
            nameof(TestDataGenerator.GetBlarggCgbSound),
            MemberType = typeof(TestDataGenerator)
        ),
        MemberData(
            nameof(TestDataGenerator.GetBlarggCpuInstrs),
            MemberType = typeof(TestDataGenerator)
        ),
        MemberData(
            nameof(TestDataGenerator.GetBlarggDmgSound2),
            MemberType = typeof(TestDataGenerator)
        ),
        MemberData(
            nameof(TestDataGenerator.GetBlarggMemTiming),
            MemberType = typeof(TestDataGenerator)
        ),
        MemberData(
            nameof(TestDataGenerator.GetBlarggOamBug),
            MemberType = typeof(TestDataGenerator)
        )
    ]
    public void Test(string rom)
    {
        RomTestUtils.TestRomWithMemory(rom);
    }
}
