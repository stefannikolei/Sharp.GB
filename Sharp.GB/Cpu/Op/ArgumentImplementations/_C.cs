using Sharp.GB.Common;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class _C : Argument
{
    public _C()
        : base("(C)", 0, true, DataType.D8)
    {
    }

    public override int read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return addressSpace.getByte(0xff00 | registers.getC());
    }

    public override void write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        addressSpace.setByte(0xff00 | registers.getC(), value);
    }
}
