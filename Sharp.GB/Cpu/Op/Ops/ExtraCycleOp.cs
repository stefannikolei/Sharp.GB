namespace Sharp.GB.Cpu.Op.Ops;

public class ExtraCycleOp() : IOp
{
    public bool ReadsMemory()
    {
        return true;
    }

    public override string ToString()
    {
        return "wait cycle";
    }
}
