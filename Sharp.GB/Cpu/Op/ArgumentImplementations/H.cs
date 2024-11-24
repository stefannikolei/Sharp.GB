using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class H : Argument
{
    public H()
        : base(nameof(H)) { }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.GetH();
    }

    public override void Write(
        Registers registers,
        IAddressSpace addressSpace,
        int[] args,
        int value
    )
    {
        registers.SetH(value);
    }
}
