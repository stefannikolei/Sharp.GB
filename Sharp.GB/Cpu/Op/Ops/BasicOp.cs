using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class BasicOp(Argument arg) : IOp
{
    public bool readsMemory()
    {
        return arg.isMemory();
    }

    public int operandLength()
    {
        return arg.getOperandLength();
    }

    public int execute(Registers registers, IAddressSpace addressSpace, int[] args, int context)
    {
        return arg.read(registers, addressSpace, args);
    }

    public override string ToString()
    {
        if (arg.getDataType() == DataType.D16)
        {
            return string.Format("%s → [__]", arg.getLabel());
        }
        else
        {
            return string.Format("%s → [_]", arg.getLabel());
        }
    }
}
