namespace Sharp.GB.Cpu.Op.Ops;

public class ProceedOp(string condition) : IOp
{
    public bool proceed(Registers registers)
    {
        switch (condition)
        {
            case "NZ":
                return !registers.getFlags().isZ();

            case "Z":
                return registers.getFlags().isZ();

            case "NC":
                return !registers.getFlags().isC();

            case "C":
                return registers.getFlags().isC();
        }

        return false;
    }

    public override string ToString()
    {
        return string.Format("? %s:", condition);
    }
}
