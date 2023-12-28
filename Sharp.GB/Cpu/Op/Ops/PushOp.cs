using System;
using Sharp.GB.Gpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class PushOp(Func<Flags, int, int> dec) : IOp
{
    public bool WritesMemory()
    {
        return true;
    }


    public int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int context)
    {
        registers.SetSp(dec.Invoke(registers.GetFlags(), registers.GetSp()));
        addressSpace.SetByte(registers.GetSp(), (context & 0xff00) >> 8);
        return context;
    }

    public SpriteBug.CorruptionType? CausesOemBug(Registers registers, int context)
    {
        return IOp.InOamArea(registers.GetSp()) ? SpriteBug.CorruptionType.Push1 : null;
    }

    public override string ToString()
    {
        return string.Format("[_ ] → (SP--)");
    }
}
