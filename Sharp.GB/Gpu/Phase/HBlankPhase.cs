namespace Sharp.GB.Gpu.Phase
{
    public class HBlankPhase : IGpuPhase
    {
        private int ticks;

        public HBlankPhase start(int ticksInLine)
        {
            this.ticks = ticksInLine;
            return this;
        }

        public bool tick()
        {
            ticks++;
            return ticks < 456;
        }
    }
}
