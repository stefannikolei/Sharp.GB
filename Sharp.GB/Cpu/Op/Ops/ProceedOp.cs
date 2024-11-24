namespace Sharp.GB.Cpu.Op.Ops;

public class ProceedOp(string condition) : IOp
{
    public bool Proceed(Registers registers)
    {
        switch (condition)
        {
            case "NZ":
                return !registers.GetFlags().IsZ();

            case "Z":
                return registers.GetFlags().IsZ();

            case "NC":
                return !registers.GetFlags().IsC();

            case "C":
                return registers.GetFlags().IsC();
        }

        return false;
    }

    public override string ToString()
    {
        return $"? {condition}:";
    }
}
