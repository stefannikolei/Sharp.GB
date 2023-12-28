using System;
using Sharp.GB.Common;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.ArgumentImplementations;

public class D16 : Argument
{
    public D16()
        : base("d16", 2, false, DataType.D16)
    {
    }

    public override int Read(Registers registers, IAddressSpace addressSpace, int[] args)
    {
        return BitUtils.ToWord(args);
    }

    public override void Write(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        throw new NotSupportedException();
    }
}
