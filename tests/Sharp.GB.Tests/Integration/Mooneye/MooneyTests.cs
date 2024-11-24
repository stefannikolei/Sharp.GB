using Xunit;
using Xunit.Abstractions;

namespace Sharp.GB.Tests.Integration.Mooneye;

public class MooneyTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public MooneyTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [
        Theory(Timeout = 100),
        // MemberData(
        //     nameof(TestDataGenerator.GetMooneyAcceptanceBits),
        //     MemberType = typeof(TestDataGenerator)
        // ),
        // MemberData(
        //     nameof(TestDataGenerator.GetMooneyAcceptanceInstr),
        //     MemberType = typeof(TestDataGenerator)
        // ),
        // MemberData(
        //     nameof(TestDataGenerator.GetMooneyAcceptanceInterrupts),
        //     MemberType = typeof(TestDataGenerator)
        // ),
        // MemberData(
        //     nameof(TestDataGenerator.GetMooneyAcceptanceoam_dma),
        //     MemberType = typeof(TestDataGenerator)
        // ),
        // MemberData(
        //     nameof(TestDataGenerator.GetMooneyAcceptancePpu),
        //     MemberType = typeof(TestDataGenerator)
        // ),
        // MemberData(
        //     nameof(TestDataGenerator.GetMooneyAcceptanceSerial),
        //     MemberType = typeof(TestDataGenerator)
        // ),
        MemberData(
            nameof(TestDataGenerator.GetMooneyAcceptanceTimer),
            MemberType = typeof(TestDataGenerator)
        )
    ]
    public void Mooney(string rom)
    {
        RomTestUtils.TestMooneyeRom(rom, _testOutputHelper);
    }
}
