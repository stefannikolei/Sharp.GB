﻿using System.Collections.Generic;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory.cart.Type
{
    public class Rom : IAddressSpace
    {
        private readonly IReadOnlyList<int> rom;

        public Rom(IReadOnlyList<int> rom, CartridgeType type, int romBanks, int ramBanks)
        {
            this.rom = rom;
        }

        public bool accepts(int address)
        {
            return (address >= 0x0000 && address < 0x8000) ||
                   (address >= 0xa000 && address < 0xc000);
        }

        public void setByte(int address, int value)
        {
        }

        public int getByte(int address)
        {
            if (address >= 0x0000 && address < 0x8000)
            {
                return rom[address];
            }

            return 0;
        }
    }
}
