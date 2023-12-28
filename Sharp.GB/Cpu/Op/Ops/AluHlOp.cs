using System;
using Sharp.GB.Cpu.OpCode;
using Sharp.GB.Gpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op.Ops;

// TODO: lastDataType probably needs to be from the execution
public class AluHlOp(Func<Flags, int, int> func) : IOp
{
    public int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int value)
    {
        return func.Invoke(registers.GetFlags(), value);
    }

    public SpriteBug.CorruptionType? CausesOemBug(Registers registers, int context)
    {
        return OpcodeBuilder.CausesOemBug(func, context) ? SpriteBug.CorruptionType.LdHl : null;
    }

    public override string ToString()
    {
        return string.Format("%s(HL) → [__]");
    }
}
