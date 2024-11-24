using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class BasicOp(Argument arg) : IOp
{
    public bool ReadsMemory()
    {
        return arg.IsMemory();
    }

    public int OperandLength()
    {
        return arg.GetOperandLength();
    }

    public int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int context)
    {
        return arg.Read(registers, addressSpace, args);
    }

    public override string ToString()
    {
        if (arg.GetDataType() == DataType.D16)
        {
            return $"{arg.GetLabel()} → [__]";
        }
        else
        {
            return $"{arg.GetLabel()} → [_]";
        }
    }
}
