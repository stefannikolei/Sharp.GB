using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sharp.GB.Common;
using Sharp.GB.Memory.cart.Battery;
using Sharp.GB.Memory.cart.Type;
using Sharp.GB.Memory.Extensions;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory.cart
{
    public class Cartridge : IAddressSpace
    {
        private static string[] s_validRomExtension = [".gb", ".gbc", ".rom"];

        private readonly IAddressSpace _addressSpace;
        private readonly string _title;
        private readonly GameboyType _gameboyType;
        private readonly bool _gbc;

        private int _dmgBoostrap;

        public Cartridge(GameboyOptions options)
        {
            var rom = LoadRom(options);
            var cartridgeType = (CartridgeType)rom[0x0147];

            _title = GetTitle(rom);
            _gameboyType = GetGameboyType(rom[0x0143]);

            var romBanks = GetRomBanks(rom[0x0148]);
            var ramBanks = GetRamBanks(rom[0x0149]);
            if (ramBanks == 0 && cartridgeType.IsRam())
            {
                ramBanks = 1;
            }

            IBattery battery;
            if (cartridgeType.IsBattery() && !options.DisableBatterySaves)
            {
                battery = new FileBattery(options.RomFileName);
            }
            else
            {
                battery = new MockBattery();
            }

            if (cartridgeType.IsMbc1())
            {
                _addressSpace = new Mbc1(rom, cartridgeType, battery, romBanks, ramBanks);
            }
            else if (cartridgeType.IsMbc2())
            {
                _addressSpace = new Mbc2(rom, cartridgeType, battery, romBanks);
            }
            else if (cartridgeType.IsMbc3())
            {
                _addressSpace = new Mbc3(rom, battery, ramBanks);
            }
            else if (cartridgeType.IsMbc5())
            {
                _addressSpace = new Mbc5(rom, cartridgeType, battery, romBanks, ramBanks);
            }
            else
            {
                _addressSpace = new Rom(rom, cartridgeType, romBanks, ramBanks);
            }

            _dmgBoostrap = options.UseBootstrap ? 0 : 1;
            if (options.ForceCgb)
            {
                _gbc = true;
            }
            else if (_gameboyType == GameboyType.NonCgb)
            {
                _gbc = false;
            }
            else if (_gameboyType == GameboyType.Cgb)
            {
                _gbc = true;
            }
            else
            {
                // UNIVERSAL
                _gbc = !options.ForceDmg;
            }
        }

        private static string GetTitle(IReadOnlyList<int> rom)
        {
            var t = new StringBuilder();
            for (var i = 0x0134; i < 0x0143; i++)
            {
                var c = (char)rom[i];
                if (c == 0)
                {
                    break;
                }

                t.Append(c);
            }

            return t.ToString();
        }

        private static IReadOnlyList<int> LoadRom(GameboyOptions options)
        {
            var extension = options.GetRomFileExtension();
            if (extension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                var zip = System.IO.Compression.ZipFile.OpenRead(options.RomFileName);

                foreach (var zipEntry in zip.Entries)
                {
                    extension = Path.GetExtension(zipEntry.Name);
                    if (!s_validRomExtension.Contains(extension))
                    {
                        continue;
                    }

                    using var stream = zipEntry.Open();
                    using var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    return Load(memoryStream.ToArray());
                }
            }
            else
            {
                return Load(options.GetRomFileContent());
            }

            throw new ArgumentException(nameof(options));
        }

        private static int[] Load(byte[] romFileContent)
        {
            var result = new int[romFileContent.Length];
            for (var i = 0; i < romFileContent.Length; i++)
            {
                result[i] = romFileContent[i] & 0xff;
            }

            return result;
        }

        private static GameboyType GetGameboyType(int value)
        {
            return value switch
            {
                0x80 => GameboyType.Universal,
                0xc0 => GameboyType.Cgb,
                _ => GameboyType.NonCgb
            };
        }

        private static int GetRomBanks(int id)
        {
            return id switch
            {
                0 => 2,
                1 => 4,
                2 => 8,
                3 => 16,
                4 => 32,
                5 => 64,
                6 => 128,
                7 => 256,
                0x52 => 72,
                0x53 => 80,
                0x54 => 96,
                _ => throw new ArgumentException("Unsupported ROM size: " + id)
            };
        }

        private static int GetRamBanks(int id)
        {
            return id switch
            {
                0 => 0,
                1 => 1,
                2 => 1,
                3 => 4,
                4 => 16,
                _ => throw new ArgumentException("Unsupported RAM size: " + id)
            };
        }

        public bool IsGbc => _gbc;

        public bool Accepts(int address)
        {
            return _addressSpace.Accepts(address) || address == 0xff50;
        }

        public void SetByte(int address, int value)
        {
            if (address == 0xff50)
            {
                _dmgBoostrap = 1;
            }
            else
            {
                _addressSpace.SetByte(address, value);
            }
        }

        public int GetByte(int address)
        {
            if (_dmgBoostrap == 0 && !_gbc && (address >= 0x0000 && address < 0x0100))
            {
                return BootRom.GameboyClassic[address];
            }
            else if (_dmgBoostrap == 0 && _gbc && address >= 0x000 && address < 0x0100)
            {
                return BootRom.GameboyColor[address];
            }
            else if (_dmgBoostrap == 0 && _gbc && address >= 0x200 && address < 0x0900)
            {
                return BootRom.GameboyColor[address - 0x0100];
            }
            else if (address == 0xff50)
            {
                return 0xff;
            }
            else
            {
                return _addressSpace.GetByte(address);
            }
        }
    }
}
