using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class _DE : Argument
{
    public _DE()
        : base("(DE)", 0, true, DataType.D8)
    {
    }

    public override int read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return addressSpace.getByte(registers.getDE());
    }

    public override void write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        addressSpace.setByte(registers.getDE(), value);
    }
}
