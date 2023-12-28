using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class Bc2 : Argument
{
    public Bc2()
        : base("(BC)", 0, true, DataType.D8)
    {
    }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return addressSpace.GetByte(registers.GetBc());
    }

    public override void Write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        addressSpace.SetByte(registers.GetBc(), value);
    }
}
