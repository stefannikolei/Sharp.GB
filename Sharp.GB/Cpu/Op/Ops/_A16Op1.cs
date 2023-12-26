using Sharp.GB.Common;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class _A16Op1(Argument arg) : IOp
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
        addressSpace.setByte(BitUtils.toWord(args), context & 0x00ff);
        return context;
    }

    public override string ToString()
    {
        return string.Format("[ _] → %s", arg.getLabel());
    }
}
