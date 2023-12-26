using System;
using Sharp.GB.Gpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

public class PushOp(Func<Flags, int, int> dec) : IOp
{
    public bool writesMemory()
    {
        return true;
    }


    public int execute(Registers registers, IAddressSpace addressSpace, int[] args, int context)
    {
        registers.setSP(dec.Invoke(registers.getFlags(), registers.getSP()));
        addressSpace.setByte(registers.getSP(), (context & 0xff00) >> 8);
        return context;
    }

    public SpriteBug.CorruptionType causesOemBug(Registers registers, int context)
    {
        return IOp.inOamArea(registers.getSP()) ? SpriteBug.CorruptionType.PUSH_1 : default;
    }

    public override string ToString()
    {
        return string.Format("[_ ] → (SP--)");
    }
}
