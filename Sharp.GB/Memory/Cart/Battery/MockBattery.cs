using System;
using System.IO;

namespace Sharp.GB.Memory.cart.Battery
{
    public class MockBattery : IBattery
    {
        public void LoadRam(int[] ram)
        {
        }

        public void SaveRam(int[] ram)
        {
        }

        public void LoadRamWithClock(int[] ram, long[] clockData)
        {
        }

        public void SaveRamWithClock(int[] ram, long[] clockData)
        {
        }
    }
}
