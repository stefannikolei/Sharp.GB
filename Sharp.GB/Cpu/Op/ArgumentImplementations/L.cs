using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class L : Argument
{
    public L()
        : base(nameof(L)) { }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.GetL();
    }

    public override void Write(
        Registers registers,
        IAddressSpace addressSpace,
        int[] args,
        int value
    )
    {
        registers.SetL(value);
    }
}
