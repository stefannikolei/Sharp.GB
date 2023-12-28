namespace Sharp.GB.Cpu.Op.Ops;

public class SwitchInterruptsOp(bool enable, bool withDelay) : IOp
{
    public void SwitchInterrupts(InterruptManager interruptManager)
    {
        if (enable)
        {
            interruptManager.EnableInterrupts(withDelay);
        }
        else
        {
            interruptManager.DisableInterrupts(withDelay);
        }
    }

    public override string ToString()
    {
        return (enable ? "enable" : "disable") + " interrupts";
    }
}
