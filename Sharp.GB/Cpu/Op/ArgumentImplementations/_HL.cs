using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class Hl2 : Argument
{
    public Hl2()
        : base("(HL)", 0, true, DataType.D8)
    {
    }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return addressSpace.GetByte(registers.GetHl());
    }

    public override void Write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        addressSpace.SetByte(registers.GetHl(), value);
    }
}
