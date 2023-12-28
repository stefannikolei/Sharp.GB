using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class De2 : Argument
{
    public De2()
        : base("(DE)", 0, true, DataType.D8)
    {
    }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return addressSpace.GetByte(registers.GetDe());
    }

    public override void Write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        addressSpace.SetByte(registers.GetDe(), value);
    }
}
