using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class Sp : Argument
{
    public Sp()
        : base("SP", 0, false, DataType.D16)
    {
    }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return registers.GetSp();
    }

    public override void Write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        registers.SetSp(value);
    }
}
