using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class A : Argument
{
    public A() : base(nameof(A))
    {
    }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.GetA();
    }

    public override void Write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        registers.SetA(value);
    }
}
