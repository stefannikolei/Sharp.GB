using Sharp.GB.Common;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class R8 : Argument
{
    public R8()
        : base("r8", 1, false, DataType.R8) { }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return BitUtils.ToSigned(args[0]);
    }

    public override void Write(
        Registers registers,
        IAddressSpace addressSpace,
        int[] args,
        int value
    )
    {
        throw new NotSupportedException();
    }
}
