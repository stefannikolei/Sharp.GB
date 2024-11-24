namespace Sharp.GB.Common
{
    public class GameboyOptions
    {
        public bool ForceDmg { get; }
        public bool ForceCgb { get; }
        public bool UseBootstrap { get; }
        public bool Debug { get; }
        public bool DisableBatterySaves { get; }
        public bool Headless { get; }
        public string RomFileName { get; }

        public GameboyOptions(string romFileName)
        {
            RomFileName = romFileName;
            DisableBatterySaves = true;
        }

        public GameboyOptions(
            string romFileName,
            ICollection<string> parameters,
            ICollection<string> shortParams
        )
        {
            RomFileName = romFileName;
            ForceDmg = parameters.Contains("force-dmg") || shortParams.Contains("d");
            ForceCgb = parameters.Contains("force-cgb") || shortParams.Contains("c");
            if (ForceDmg && ForceCgb)
            {
                throw new ArgumentException(
                    "force-dmg and force-cgb options are can't be used together"
                );
            }

            UseBootstrap = parameters.Contains("use-bootstrap") || shortParams.Contains("b");
            DisableBatterySaves =
                parameters.Contains("disable-battery-saves") || shortParams.Contains("db");
            Debug = parameters.Contains("debug");
            Headless = parameters.Contains("headless");
        }

        public byte[] GetRomFileContent()
        {
            if (!File.Exists(RomFileName))
            {
                throw new FileNotFoundException("Rom not found" + RomFileName);
            }

            using var stream = File.OpenRead(RomFileName);
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }

        public string GetRomFileExtension()
        {
            if (!File.Exists(RomFileName))
            {
                throw new FileNotFoundException("Rom not found" + RomFileName);
            }

            return Path.GetExtension(RomFileName);
        }
    }
}
