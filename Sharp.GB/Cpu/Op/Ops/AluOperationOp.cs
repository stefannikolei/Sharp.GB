using Sharp.GB.Cpu.OpCode;
using Sharp.GB.Gpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class AluOperationOp(Func<Flags, int, int> func, string operation, DataType lastDataType)
    : IOp
{
    public int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        return func.Invoke(registers.GetFlags(), value);
    }

    public SpriteBug.CorruptionType? CausesOemBug(Registers registers, int context)
    {
        return OpcodeBuilder.CausesOemBug(func, context) ? SpriteBug.CorruptionType.IncDec : null;
    }

    public override string ToString()
    {
        if (lastDataType == DataType.D16)
        {
            return $"{operation}([__]) → [__]";
        }
        else
        {
            return $"{operation}([_]) → [_]";
        }
    }
}
