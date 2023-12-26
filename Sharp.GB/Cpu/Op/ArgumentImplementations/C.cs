using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class C : Argument
{
    public C()
        : base(nameof(C))
    {
    }

    public override int read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.getC();
    }

    public override void write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        registers.setC(value);
    }
}
