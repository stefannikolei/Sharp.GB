using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class _HL : Argument
{
    public _HL()
        : base("(HL)", 0, true, DataType.D8)
    {
    }

    public override int read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return addressSpace.getByte(registers.getHL());
    }

    public override void write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        addressSpace.setByte(registers.getHL(), value);
    }
}
