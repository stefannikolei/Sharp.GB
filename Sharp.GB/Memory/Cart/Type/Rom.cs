using System.Collections.Generic;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory.cart.Type
{
    public class Rom : IAddressSpace
    {
        private readonly IReadOnlyList<int> _rom;

        public Rom(IReadOnlyList<int> rom, CartridgeType type, int romBanks, int ramBanks)
        {
            _rom = rom;
        }

        public bool Accepts(int address)
        {
            return (address >= 0x0000 && address < 0x8000) ||
                   (address >= 0xa000 && address < 0xc000);
        }

        public void SetByte(int address, int value)
        {
        }

        public int GetByte(int address)
        {
            if (address >= 0x0000 && address < 0x8000)
            {
                return _rom[address];
            }

            return 0;
        }
    }
}
