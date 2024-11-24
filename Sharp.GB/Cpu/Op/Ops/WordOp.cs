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
        return $"0x{value:X2} → [__]";
    }
}
