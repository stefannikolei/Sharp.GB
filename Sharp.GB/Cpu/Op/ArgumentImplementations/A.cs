using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class A : Argument
{
    public A() : base(nameof(A))
    {
    }

    public override int read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.getA();
    }

    public override void write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        registers.setA(value);
    }
}
