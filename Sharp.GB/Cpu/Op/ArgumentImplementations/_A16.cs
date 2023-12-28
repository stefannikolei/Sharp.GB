using Sharp.GB.Common;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class A162 : Argument
{
    public A162()
        : base("(a16)", 2, true, DataType.D8)
    {
    }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return addressSpace.GetByte(BitUtils.ToWord(args));
    }

    public override void Write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        addressSpace.SetByte(BitUtils.ToWord(args), value);
    }
}
