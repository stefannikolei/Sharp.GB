using System;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory
{
    public class DmaAddressSpace : IAddressSpace
    {
        private readonly IAddressSpace addressSpace;

        public DmaAddressSpace(IAddressSpace addressSpace)
        {
            this.addressSpace = addressSpace;
        }

        public bool accepts(int address)
        {
            return true;
        }

        public void setByte(int address, int value)
        {
            throw new NotSupportedException();
        }

        public int getByte(int address)
        {
            if (address < 0xe000)
            {
                return addressSpace.getByte(address);
            }
            else
            {
                return addressSpace.getByte(address - 0x2000);
            }
        }
    }
}
