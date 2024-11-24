using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class WriteOp(Argument arg) : IOp
{
    public bool WritesMemory()
    {
        return arg.IsMemory();
    }

    public int OperandLength()
    {
        return arg.GetOperandLength();
    }

    public int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int context)
    {
        arg.Write(registers, addressSpace, args, context);
        return context;
    }

    public override string ToString()
    {
        if (arg.GetDataType() == DataType.D16)
        {
            return $"[__] → {arg.GetLabel()}";
        }
        else
        {
            return $"[_] → {arg.GetLabel()}";
        }
    }
}
