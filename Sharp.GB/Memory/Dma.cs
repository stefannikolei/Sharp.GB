using Sharp.GB.Cpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory
{
    public class Dma : IAddressSpace
    {
        private readonly IAddressSpace addressSpace;

        private readonly IAddressSpace oam;

        private readonly SpeedMode speedMode;

        private bool transferInProgress;

        private bool restarted;

        private int from;

        private int ticks;

        private int regValue = 0xff;

        public Dma(IAddressSpace addressSpace, IAddressSpace oam, SpeedMode speedMode)
        {
            this.addressSpace = new DmaAddressSpace(addressSpace);
            this.speedMode = speedMode;
            this.oam = oam;
        }

        public bool accepts(int address)
        {
            return address == 0xff46;
        }

        public void Tick()
        {
            if (transferInProgress)
            {
                if (++ticks >= 648 / speedMode.GetSpeedMode())
                {
                    transferInProgress = false;
                    restarted = false;
                    ticks = 0;
                    for (int i = 0; i < 0xa0; i++)
                    {
                        oam.setByte(0xfe00 + i, addressSpace.getByte(from + i));
                    }
                }
            }
        }


        public void setByte(int address, int value)
        {
            from = value * 0x100;
            restarted = IsOamBlocked();
            ticks = 0;
            transferInProgress = true;
            regValue = value;
        }


        public int getByte(int address)
        {
            return regValue;
        }

        public bool IsOamBlocked()
        {
            return restarted || (transferInProgress && ticks >= 5);
        }
    }
}
