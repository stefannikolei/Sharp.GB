using Sharp.GB.Common;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class _A16Op2(Argument arg) : IOp
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
        addressSpace.setByte((BitUtils.toWord(args) + 1) & 0xffff, (context & 0xff00) >> 8);
        return context;
    }

    public override string ToString()
    {
        return string.Format("[_ ] → %s", arg.getLabel());
    }
}
