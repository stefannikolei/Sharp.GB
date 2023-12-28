namespace Sharp.GB.Cpu.Op.Ops;

public class ForceFinishOp() : IOp
{
    public bool ForceFinishCycle()
    {
        return true;
    }

    public override string ToString()
    {
        return "finish cycle";
    }
}
