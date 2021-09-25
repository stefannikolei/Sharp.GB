namespace Sharp.GB.Gpu.Phase
{
    public class VBlankPhase : IGpuPhase
    {
        private int ticks;

        public VBlankPhase start()
        {
            ticks = 0;
            return this;
        }

        public bool tick()
        {
            return ++ticks < 456;
        }
    }
}
