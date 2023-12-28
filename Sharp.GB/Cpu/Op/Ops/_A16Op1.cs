using Sharp.GB.Common;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class A16Op1(Argument arg) : IOp
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
        addressSpace.SetByte(BitUtils.ToWord(args), context & 0x00ff);
        return context;
    }

    public override string ToString()
    {
        return string.Format("[ _] → %s", arg.GetLabel());
    }
}
