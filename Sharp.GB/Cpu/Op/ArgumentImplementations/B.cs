using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class B : Argument
{
    public B()
        : base(nameof(B))
    {
    }

    public override int read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.getB();
    }

    public override void write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        registers.setB(value);
    }
}
