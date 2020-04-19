using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sharp.GB.Common;
using Sharp.GB.Memory.Extensions;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory.cart
{
    public class Cartridge : IAddressSpace
    {
        private static string[] s_ValidRomExtension = {".gb", ".gbc", ".rom"};

        private readonly string _title;
        private readonly GameboyType _gameboyType;

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
                    if (!s_ValidRomExtension.Contains(extension))
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
    }
}
