using Sharp.GB.Cpu;
using Sharp.GB.Cpu.OpCode;
using Sharp.GB.Memory;
using Sharp.GB.Memory.Interface;
using Xunit;

public class TimingTest
{
    private const int Offset = 0x100;

    private readonly Cpu _cpu;
    private readonly IAddressSpace _memory;

    public TimingTest()
    {
        _memory = new Ram(0x00, 0x10000);
        _cpu = new Cpu(
            _memory,
            new InterruptManager(false),
            null,
            NullDisplay.Instance,
            new SpeedMode()
        );
    }

    [Fact]
    public void TestTiming()
    {
        AssertTiming(16, [0xc9, 0, 0]); // RET
        AssertTiming(16, [0xd9, 0, 0]); // RETI
        _cpu.GetRegisters().GetFlags().SetZ(false);
        AssertTiming(20, [0xc0, 0, 0]); // RET NZ
        _cpu.GetRegisters().GetFlags().SetZ(true);
        AssertTiming(8, [0xc0, 0, 0]); // RET NZ
        AssertTiming(24, [0xcd, 0, 0]); // CALL a16
        AssertTiming(16, [0xc5]); // PUSH BC
        AssertTiming(12, [0xf1]); // POP AF

        // -- Rest of the code is omitted for brevity
    }

    private void AssertTiming(int expectedTiming, int[] opcodes)
    {
        for (int i = 0; i < opcodes.Length; i++)
        {
            _memory.SetByte(Offset + i, opcodes[i]);
        }
        _cpu.ClearState();
        _cpu.GetRegisters().SetPc(Offset);

        int ticks = 0;
        Opcode? opcode = null;
        do
        {
            _cpu.Tick();

            if (opcode == null && _cpu.GetCurrentOpcode() != null)
            {
                opcode = _cpu.GetCurrentOpcode();
            }
            ticks++;
        } while (_cpu.GetState() != Cpu.State.Opcode || ticks < 4);

        var errorText = "Invalid timing value for " + HexArray(opcodes);
        Assert.Equal(expectedTiming, ticks);

        // opcode == null ? errorText : errorText + "[" + opcode.ToString() + "]";
    }

    private static string HexArray(int[] data)
    {
        return "[" + string.Join(" ", data.Select(x => x.ToString("x2"))) + "]";
    }
}
