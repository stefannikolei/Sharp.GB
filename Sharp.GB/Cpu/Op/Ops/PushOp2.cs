using System;
using Sharp.GB.Gpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class PushOp2(Func<Flags, int, int> dec) : IOp
{
    public bool WritesMemory()
    {
        return true;
    }

    public int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int context)
    {
        registers.SetSp(dec.Invoke(registers.GetFlags(), registers.GetSp()));
        addressSpace.SetByte(registers.GetSp(), context & 0x00ff);
        return context;
    }

    public SpriteBug.CorruptionType? CausesOemBug(Registers registers, int context)
    {
        return IOp.InOamArea(registers.GetSp()) ? SpriteBug.CorruptionType.Push2 : null;
    }

    public override string ToString()
    {
        return "[ _] → (SP--)";
    }
}
