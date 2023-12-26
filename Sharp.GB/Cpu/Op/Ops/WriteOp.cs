using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class WriteOp(Argument arg) : IOp
{
    public bool writesMemory()
    {
        return arg.isMemory();
    }

    public int operandLength()
    {
        return arg.getOperandLength();
    }

    public int execute(Registers registers, IAddressSpace addressSpace, int[] args, int context)
    {
        arg.write(registers, addressSpace, args, context);
        return context;
    }

    public override string ToString()
    {
        if (arg.getDataType() == DataType.D16)
        {
            return string.Format("[__] → %s", arg.getLabel());
        }
        else
        {
            return string.Format("[_] → %s", arg.getLabel());
        }
    }
}
