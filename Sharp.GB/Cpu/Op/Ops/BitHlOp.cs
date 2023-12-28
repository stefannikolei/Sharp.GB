using Sharp.GB.Common;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class BitHlOp(int bit) : IOp
{
    public bool ReadsMemory()
    {
        return true;
    }

    public int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int context)
    {
        int value = addressSpace.GetByte(registers.GetHl());
        Flags flags = registers.GetFlags();
        flags.SetN(false);
        flags.SetH(true);
        if (bit < 8)
        {
            flags.SetZ(!BitUtils.GetBit(value, bit));
        }

        return context;
    }

    public override string ToString()
    {
        return string.Format("BIT(%d,HL)", bit);
    }
}
