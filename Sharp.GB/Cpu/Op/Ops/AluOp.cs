using System;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

// TODO: lastDataType probably needs to be from the execution
public class AluOp(Func<Flags, int, int, int> func, Argument arg2, string operation, DataType lastDataType) : IOp
{
    public bool readsMemory()
    {
        return arg2.isMemory();
    }

    public int operandLength()
    {
        return arg2.getOperandLength();
    }

    public int execute(Registers registers, IAddressSpace addressSpace, int[] args, int v1)
    {
        int v2 = arg2.read(registers, addressSpace, args);
        return func.Invoke(registers.getFlags(), v1, v2);
    }

    public override string ToString()
    {
        if (lastDataType == DataType.D16)
        {
            return String.Format("%s([__],%s) → [__]", operation, arg2);
        }
        else
        {
            return String.Format("%s([_],%s) → [_]", operation, arg2);
        }
    }
}
