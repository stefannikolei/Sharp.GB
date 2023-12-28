using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class E : Argument
{
    public E()
        : base(nameof(E))
    {
    }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.GetE();
    }

    public override void Write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        registers.SetE(value);
    }
}
