using System.Collections.Generic;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory
{
    public class Mmu : IAddressSpace
    {
        private readonly List<IAddressSpace> _spaces = new List<IAddressSpace>();

        public void AddAddressSpace(IAddressSpace space)
        {
            _spaces.Add(space);
        }
        
        public bool accepts(int address)
        {
            return true;
        }

        public void setByte(int address, int value)
        {
            GetSpace(address).setByte(address, value);
        }

        public int getByte(int address)
        {
            return GetSpace(address).getByte(address);
        }

        private IAddressSpace GetSpace(int address)
        {
            foreach (var space in _spaces)
            {
                if (space.accepts(address))
                {
                    return space;
                }
            }

            return null;
        }
    }
}
