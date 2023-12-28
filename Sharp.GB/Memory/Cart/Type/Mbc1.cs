using System;
using System.Collections.Generic;
using Sharp.GB.Memory.cart.Battery;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory.cart.Type
{
    public class Mbc1 : IAddressSpace
    {
        private static readonly int[] s_nintendoLogo =
        [
            0xCE, 0xED, 0x66, 0x66, 0xCC, 0x0D, 0x00, 0x0B, 0x03, 0x73, 0x00, 0x83, 0x00, 0x0C, 0x00, 0x0D, 0x00,
            0x08, 0x11, 0x1F, 0x88, 0x89, 0x00, 0x0E, 0xDC, 0xCC, 0x6E, 0xE6, 0xDD, 0xDD, 0xD9, 0x99, 0xBB, 0xBB,
            0x67, 0x63, 0x6E, 0x0E, 0xEC, 0xCC, 0xDD, 0xDC, 0x99, 0x9F, 0xBB, 0xB9, 0x33, 0x3E
        ];

        private readonly bool _multiCart;
        private readonly IReadOnlyList<int> _cartRidge;
        private readonly int _romBanks;
        private readonly int _ramBanks;
        private readonly int[] _ram;
        private readonly CartridgeType _cartRidgeType;
        private readonly IBattery? _battery;

        private bool _ramWriteEnabled;
        private int _memoryModel;
        private int _selectedRamBank;
        private int _selectedRomBank = 1;
        private int _cachedRomBankFor0X0000 = -1;
        private int _cachedRomBankFor0X4000 = -1;

        public Mbc1(IReadOnlyList<int> cartridge, CartridgeType type, IBattery battery, int romBanks, int ramBanks)
        {
            _multiCart = romBanks == 64 && IsMultiCart(cartridge);
            _cartRidge = cartridge;
            _ramBanks = ramBanks;
            _romBanks = romBanks;

            _ram = new int[0x2000 * _ramBanks];
            for (var i = 0; i < _ram.Length; i++)
            {
                _ram[i] = 0xff;
            }

            _cartRidgeType = type;
            _battery = battery;
            _battery?.LoadRam(_ram);
        }

        public bool Accepts(int address)
        {
            return address >= 0x0000 && address < 0x8000 ||
                   address >= 0xa000 && address < 0xc000;
        }

        public void SetByte(int address, int value)
        {
            if (address > 0x0000 && address < 0x2000)
            {
                _ramWriteEnabled = (value & 0b1111) == 0b1010;
                if (!_ramWriteEnabled)
                {
                    _battery?.SaveRam(_ram);
                }
            }
            else if (address >= 0x2000 && address < 0x4000)
            {
                var bank = _selectedRomBank & 0b01100000;
                _selectedRomBank = bank | (value & 0b00011111);

                _cachedRomBankFor0X0000 = _cachedRomBankFor0X4000 = -1;
            }
            else if (address >= 0x4000 && address < 0x6000 && _memoryModel == 0)
            {
                var bank = _selectedRomBank & 0b00011111;
                _selectedRomBank = bank | ((value & 0b11) << 5);

                _cachedRomBankFor0X0000 = _cachedRomBankFor0X4000 = -1;
            }
            else if (address >= 0x4000 && address < 0x6000 && _memoryModel == 1)
            {
                _selectedRamBank = value & 0b11;

                _cachedRomBankFor0X0000 = _cachedRomBankFor0X4000 = -1;
            }
            else if (address >= 0x6000 && address < 0x8000)
            {
                _memoryModel = value & 1;
                _cachedRomBankFor0X0000 = _cachedRomBankFor0X4000 = -1;
            }
            else if (address >= 0xa000 && address < 0xc000 && _ramWriteEnabled)
            {
                int ramAddress = GetRamAddress(address);
                if (ramAddress < _ram.Length)
                {
                    _ram[ramAddress] = value;
                }
            }
        }

        public int GetByte(int address)
        {
            if (address >= 0x0000 && address < 0x4000)
            {
                return GetRomByte(GetRomBankFor0X0000(), address);
            }
            else if (address >= 0x4000 && address < 0x8000)
            {
                return GetRomByte(GetRomBankFor0X4000(), address - 0x4000);
            }
            else if (address >= 0xa000 && address < 0xc000)
            {
                if (_ramWriteEnabled)
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
            else
            {
                // TODOint ToHexString
                throw new ArgumentException(address.ToString());
            }
        }

        private int GetRomBankFor0X0000()
        {
            if (_cachedRomBankFor0X0000 == -1)
            {
                if (_memoryModel == 0)
                {
                    _cachedRomBankFor0X0000 = 0;
                }
                else
                {
                    int bank = _selectedRamBank << 5;
                    if (_multiCart)
                    {
                        bank >>= 1;
                    }

                    bank %= _romBanks;
                    _cachedRomBankFor0X0000 = bank;
                }
            }

            return _cachedRomBankFor0X0000;
        }

        private int GetRomBankFor0X4000()
        {
            if (_cachedRomBankFor0X4000 == -1)
            {
                int bank = _selectedRomBank;
                if (bank % 0x20 == 0)
                {
                    bank++;
                }

                if (_memoryModel == 1)
                {
                    bank &= 0b00011111;
                    bank |= _selectedRamBank << 5;
                }

                if (_multiCart)
                {
                    bank = ((bank >> 1) & 0x30) | (bank & 0x0f);
                }

                bank %= _romBanks;
                _cachedRomBankFor0X4000 = bank;
            }

            return _cachedRomBankFor0X4000;
        }

        private int GetRomByte(int bank, int address)
        {
            int cartOffset = bank * 0x4000 + address;
            if (cartOffset < _cartRidge.Count)
            {
                return _cartRidge[cartOffset];
            }
            else
            {
                return 0xff;
            }
        }

        private int GetRamAddress(in int address)
        {
            if (_memoryModel == 0)
            {
                return address - 0xa000;
            }
            else
            {
                return _selectedRamBank % _ramBanks * 0x2000 + (address - 0xa000);
            }
        }

        private static bool IsMultiCart(IReadOnlyList<int> rom)
        {
            var logoCount = 0;
            for (var i = 0; i < rom.Count; i += 0x4000)
            {
                var logoMatches = true;
                for (var j = 0; j < s_nintendoLogo.Length; j++)
                {
                    if (rom[i + 0x104 + j] == s_nintendoLogo[j])
                    {
                        continue;
                    }

                    logoMatches = false;
                    break;
                }

                if (logoMatches)
                {
                    logoCount++;
                }
            }

            return logoCount > 1;
        }
    }
}
