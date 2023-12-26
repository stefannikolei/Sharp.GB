using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class _A8 : Argument
{
    public _A8()
        : base("(a8)", 1, true, DataType.D8)
    {
    }

    public override int read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return addressSpace.getByte(0xff00 | args[0]);
    }

    public override void write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        addressSpace.setByte(0xff00 | args[0], value);
    }
}
