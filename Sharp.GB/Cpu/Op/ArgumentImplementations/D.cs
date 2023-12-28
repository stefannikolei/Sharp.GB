using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class D : Argument
{
    public D()
        : base(nameof(D))
    {
    }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.GetD();
    }

    public override void Write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        registers.SetD(value);
    }
}
