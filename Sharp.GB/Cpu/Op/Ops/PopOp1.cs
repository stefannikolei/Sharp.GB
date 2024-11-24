using Sharp.GB.Gpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class PopOp1(Func<Flags, int, int> dec) : IOp
{
    public bool ReadsMemory()
    {
        return true;
    }

    public int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int context)
    {
        int lsb = addressSpace.GetByte(registers.GetSp());
        registers.SetSp(dec.Invoke(registers.GetFlags(), registers.GetSp()));
        return lsb;
    }

    public SpriteBug.CorruptionType? CausesOemBug(Registers registers, int context)
    {
        return IOp.InOamArea(registers.GetSp()) ? SpriteBug.CorruptionType.Pop1 : null;
    }

    public override string ToString()
    {
        return "(SP++) → [ _]";
    }
}
