using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu
{
    public class SpeedMode : IAddressSpace
    {
        private bool currentSpeed;

        private bool prepareSpeedSwitch;

        public bool accepts(int address)
        {
            return address == 0xff4d;
        }

        public void setByte(int address, int value)
        {
            prepareSpeedSwitch = (value & 0x01) != 0;
        }

        public int getByte(int address)
        {
            return (currentSpeed ? (1 << 7) : 0) | (prepareSpeedSwitch ? (1 << 0) : 0) | 0b01111110;
        }

        bool OnStop()
        {
            if (prepareSpeedSwitch)
            {
                currentSpeed = !currentSpeed;
                prepareSpeedSwitch = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetSpeedMode()
        {
            return currentSpeed ? 2 : 1;
        }
    }
}
