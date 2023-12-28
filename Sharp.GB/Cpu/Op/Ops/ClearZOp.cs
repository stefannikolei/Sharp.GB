using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class ClearZOp() : IOp
{
    public int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int context)
    {
        registers.GetFlags().SetZ(false);
        return context;
    }

    public override string ToString()
    {
        return string.Format("0 → Z");
    }
}
