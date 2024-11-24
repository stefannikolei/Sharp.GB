using System;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class D8 : Argument
{
    public D8()
        : base("d8", 1, false, DataType.D8) { }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return args[0];
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
