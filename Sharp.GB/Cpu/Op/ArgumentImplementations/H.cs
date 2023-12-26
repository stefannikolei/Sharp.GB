using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class H : Argument
{
    public H()
        : base(nameof(H))
    {
    }

    public override int read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.getH();
    }

    public override void write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        registers.setH(value);
    }
}
