using Sharp.GB.Common;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class A16Op2(Argument arg) : IOp
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
        addressSpace.SetByte((BitUtils.ToWord(args) + 1) & 0xffff, (context & 0xff00) >> 8);
        return context;
    }

    public override string ToString()
    {
        return $"[_ ] → {arg.GetLabel()}";
    }
}
