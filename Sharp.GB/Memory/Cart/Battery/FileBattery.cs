using System;
using System.IO;

namespace Sharp.GB.Memory.cart.Battery
{
    public class FileBattery : IBattery
    {
        private readonly string _saveFilePath;

        public FileBattery(string romFileName)
        {
            _saveFilePath = Path.GetFullPath(romFileName) + Path.GetFileNameWithoutExtension(romFileName) + ".sav";
        }

        public void LoadRam(int[] ram)
        {
            LoadRamWithClock(ram, null);
        }

        public void SaveRam(int[] ram)
        {
            SaveRamWithClock(ram, null);
        }

        public void LoadRamWithClock(int[] ram, long[]? clockData)
        {
            if (!File.Exists(_saveFilePath))
            {
                return;
            }

            using var stream = File.OpenRead(_saveFilePath);
            var saveLength = stream.Length;
            saveLength = saveLength - saveLength % 0x2000;
            LoadRam(ram, stream, saveLength);
            if (clockData != null)
            {
                LoadClock(clockData, stream);
            }
        }

        private void LoadClock(long[] clockData, FileStream stream)
        {
            throw new NotImplementedException();
        }


        private void SaveClock(long[] clockData)
        {
            throw new NotImplementedException();
        }

        private void LoadRam(int[] ram, FileStream stream, in long saveLength)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);

            var buffer = ms.ToArray().AsSpan();

            for (var i = 0; i < ram.Length; i++)
            {
                ram[i] = buffer[i] & 0xff;
            }
        }

        public void SaveRamWithClock(int[] ram, long[]? clockData)
        {
            var buffer = new byte[ram.Length];
            for (var i = 0; i < ram.Length; i++)
            {
                buffer[i] = (byte)ram[i];
            }

            File.WriteAllBytes(_saveFilePath, buffer);
            if (clockData != null)
            {
                SaveClock(clockData);
            }
        }
    }
}
