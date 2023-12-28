namespace Sharp.GB.Gpu.Phase
{
    public class VBlankPhase : IGpuPhase
    {
        private int _ticks;

        public VBlankPhase Start()
        {
            _ticks = 0;
            return this;
        }

        public bool Tick()
        {
            return ++_ticks < 456;
        }
    }
}
