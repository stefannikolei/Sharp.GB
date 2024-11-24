using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory
{
    public class DmaAddressSpace : IAddressSpace
    {
        private readonly IAddressSpace _addressSpace;

        public DmaAddressSpace(IAddressSpace addressSpace)
        {
            _addressSpace = addressSpace;
        }

        public bool Accepts(int address)
        {
            return true;
        }

        public void SetByte(int address, int value)
        {
            throw new NotSupportedException();
        }

        public int GetByte(int address)
        {
            if (address < 0xe000)
            {
                return _addressSpace.GetByte(address);
            }
            else
            {
                return _addressSpace.GetByte(address - 0x2000);
            }
        }
    }
}
