using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class D : Argument
{
    public D()
        : base(nameof(D))
    {
    }

    public override int read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.getD();
    }

    public override void write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        registers.setD(value);
    }
}
