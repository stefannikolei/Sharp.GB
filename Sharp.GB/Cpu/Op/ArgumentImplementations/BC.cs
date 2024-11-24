using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class Bc : Argument
{
    public Bc()
        : base("BC", 0, false, DataType.D16) { }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.GetBc();
    }

    public override void Write(
        Registers registers,
        IAddressSpace addressSpace,
        int[] args,
        int value
    )
    {
        registers.SetBc(value);
    }
}
