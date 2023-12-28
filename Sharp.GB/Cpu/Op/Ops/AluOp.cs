using System;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

// TODO: lastDataType probably needs to be from the execution
public class AluOp(Func<Flags, int, int, int> func, Argument arg2, string operation, DataType lastDataType) : IOp
{
    public bool ReadsMemory()
    {
        return arg2.IsMemory();
    }

    public int OperandLength()
    {
        return arg2.GetOperandLength();
    }

    public int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int v1)
    {
        int v2 = arg2.Read(registers, addressSpace, args);
        return func.Invoke(registers.GetFlags(), v1, v2);
    }

    public override string ToString()
    {
        if (lastDataType == DataType.D16)
        {
            return string.Format("%s([__],%s) → [__]", operation, arg2);
        }
        else
        {
            return string.Format("%s([_],%s) → [_]", operation, arg2);
        }
    }
}
