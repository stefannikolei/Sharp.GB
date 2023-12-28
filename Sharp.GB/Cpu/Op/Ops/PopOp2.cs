using System;
using Sharp.GB.Gpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class PopOp2(Func<Flags, int, int> dec) : IOp
{
    public bool ReadsMemory()
    {
        return true;
    }

    public int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int context)
    {
        int msb = addressSpace.GetByte(registers.GetSp());
        registers.SetSp(dec.Invoke(registers.GetFlags(), registers.GetSp()));
        return context | (msb << 8);
    }

    public SpriteBug.CorruptionType? CausesOemBug(Registers registers, int context)
    {
        return IOp.InOamArea(registers.GetSp()) ? SpriteBug.CorruptionType.Pop2 : null;
    }

    public override string ToString()
    {
        return string.Format("(SP++) → [_ ]");
    }
}
