using System;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class AluD8Op(Func<Flags, int, int, int> func, string operation, int d8Value) : IOp
{
    public int execute(Registers registers, IAddressSpace addressSpace, int[] args, int v1)
    {
        return func.Invoke(registers.getFlags(), v1, d8Value);
    }

    public override string ToString()
    {
        return String.Format("%s(%d,[_]) → [_]", operation, d8Value);
    }
}
