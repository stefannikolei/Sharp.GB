using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class Hl : Argument
{
    public Hl()
        : base("HL", 0, false, DataType.D16)
    {
    }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.GetHl();
    }

    public override void Write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        registers.SetHl(value);
    }
}
