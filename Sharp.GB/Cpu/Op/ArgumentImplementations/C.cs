using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class C : Argument
{
    public C()
        : base(nameof(C)) { }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.GetC();
    }

    public override void Write(
        Registers registers,
        IAddressSpace addressSpace,
        int[] args,
        int value
    )
    {
        registers.SetC(value);
    }
}
