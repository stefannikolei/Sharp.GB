using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class WordOp(int value) : IOp
{
    public int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int context)
    {
        return value;
    }

    public override string ToString()
    {
        return string.Format("0x%02X → [__]", value);
    }
}
