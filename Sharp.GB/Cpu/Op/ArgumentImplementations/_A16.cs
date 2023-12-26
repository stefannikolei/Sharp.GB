using Sharp.GB.Common;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class _A16 : Argument
{
    public _A16()
        : base("(a16)", 2, true, DataType.D8)
    {
    }

    public override int read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return addressSpace.getByte(BitUtils.toWord(args));
    }

    public override void write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        addressSpace.setByte(BitUtils.toWord(args), value);
    }
}
