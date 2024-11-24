using System.Collections.Generic;
using Sharp.GB.Memory.cart.Battery;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory.cart.Type
{
    public class Mbc2 : IAddressSpace
    {
        private IReadOnlyList<int> _cartridge;
        private int _romBanks;
        private int[] _ram;
        private CartridgeType _type;
        private IBattery _battery;

        private int _selectedRomBank = 1;

        private bool _ramWriteEnabled;

        public Mbc2(
            IReadOnlyList<int> cartridge,
            CartridgeType type,
            IBattery battery,
            int romBanks
        )
        {
            _cartridge = cartridge;
            _romBanks = romBanks;
            _ram = new int[0x0200];

            for (int i = 0; i < _ram.Length; i++)
            {
                _ram[i] = 0xff;
            }

            _type = type;
            _battery = battery;
            _battery.LoadRam(_ram);
        }

        public bool Accepts(int address)
        {
            return address >= 0x0000 && address < 0x8000 || address >= 0xa000 && address < 0xc000;
        }

        public void SetByte(int address, int value)
        {
            if (address >= 0x0000 && address < 0x2000)
            {
                if ((address & 0x0100) == 0)
                {
                    _ramWriteEnabled = (value & 0b1010) != 0;
                    if (!_ramWriteEnabled)
                    {
                        _battery.SaveRam(_ram);
                    }
                }
            }
            else if (address >= 0x2000 && address < 0x4000)
            {
                if ((address & 0x0100) != 0)
                {
                    _selectedRomBank = value & 0b00001111;
                }
            }
            else if (address >= 0xa000 && address < 0xc000 && _ramWriteEnabled)
            {
                int ramAddress = GetRamAddress(address);
                if (ramAddress < _ram.Length)
                {
                    _ram[ramAddress] = value & 0x0f;
                }
            }
        }

        private int GetRamAddress(int address)
        {
            return address - 0xa000;
        }

        public int GetByte(int address)
        {
            if (address >= 0x0000 && address < 0x4000)
            {
                return GetRomByte(0, address);
            }
            else if (address >= 0x4000 && address < 0x8000)
            {
                return GetRomByte(_selectedRomBank, address - 0x4000);
            }
            else if (address >= 0xa000 && address < 0xb000)
            {
                int ramAddress = GetRamAddress(address);
                if (ramAddress < _ram.Length)
                {
                    return _ram[ramAddress];
                }
                else
                {
                    return 0xff;
                }
            }
            else
            {
                return 0xff;
            }
        }

        private int GetRomByte(int bank, int address)
        {
            int cartOffset = bank * 0x4000 + address;
            if (cartOffset < _cartridge.Count)
            {
                return _cartridge[cartOffset];
            }
            else
            {
                return 0xff;
            }
        }
    }
}
