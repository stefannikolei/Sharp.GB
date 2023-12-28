using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class C2 : Argument
{
    public C2()
        : base("(C)", 0, true, DataType.D8)
    {
    }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return addressSpace.GetByte(0xff00 | registers.GetC());
    }

    public override void Write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        addressSpace.SetByte(0xff00 | registers.GetC(), value);
    }
}
