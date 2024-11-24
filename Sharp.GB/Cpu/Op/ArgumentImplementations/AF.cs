using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class Af : Argument
{
    public Af()
        : base("AF", 0, false, DataType.D16) { }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.GetAf();
    }

    public override void Write(
        Registers registers,
        IAddressSpace addressSpace,
        int[] args,
        int value
    )
    {
        registers.SetAf(value);
    }
}
