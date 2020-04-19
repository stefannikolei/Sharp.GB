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
        
        public bool Accepts(int address)
        {
            return true;
        }

        public void SetByte(int address, int value)
        {
            GetSpace(address).SetByte(address, value);
        }

        public int GetByte(int address)
        {
            return GetSpace(address).GetByte(address);
        }

        private IAddressSpace GetSpace(int address)
        {
            foreach (var space in _spaces)
            {
                if (space.Accepts(address))
                {
                    return space;
                }
            }

            return null;
        }
    }
}
