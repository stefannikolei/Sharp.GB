using System.IO;

namespace Sharp.GB.Common
{
    public class GameboyOptions
    {
        public string RomFileName { get; }

        public GameboyOptions(string romFileName)
        {
            RomFileName = romFileName;
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
