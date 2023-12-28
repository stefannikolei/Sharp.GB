namespace Sharp.GB.Gpu
{
    public class TileAttributes
    {
        public static readonly TileAttributes Empty;

        private static readonly TileAttributes[] s_attributes;

        static TileAttributes()
        {
            s_attributes = new TileAttributes[256];
            for (int i = 0; i < 256; i++)
            {
                s_attributes[i] = new TileAttributes(i);
            }

            Empty = s_attributes[0];
        }

        private readonly int _value;

        private TileAttributes(int value)
        {
            this._value = value;
        }

        public static TileAttributes ValueOf(int value)
        {
            return s_attributes[value];
        }

        public bool IsPriority()
        {
            return (_value & (1 << 7)) != 0;
        }

        public bool IsYflip()
        {
            return (_value & (1 << 6)) != 0;
        }

        public bool IsXflip()
        {
            return (_value & (1 << 5)) != 0;
        }

        public GpuRegister GetDmgPalette()
        {
            return (_value & (1 << 4)) == 0 ? GpuRegister.Obp0 : GpuRegister.Obp1;
        }

        public int GetBank()
        {
            return (_value & (1 << 3)) == 0 ? 0 : 1;
        }

        public int GetColorPaletteIndex()
        {
            return _value & 0x07;
        }
    }
}
