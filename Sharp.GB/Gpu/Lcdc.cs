using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Gpu
{
    public class Lcdc : IAddressSpace
    {
        private int _value = 0x91;

        public bool IsBgAndWindowDisplay()
        {
            return (_value & 0x01) != 0;
        }

        public bool IsObjDisplay()
        {
            return (_value & 0x02) != 0;
        }

        public int GetSpriteHeight()
        {
            return (_value & 0x04) == 0 ? 8 : 16;
        }

        public int GetBgTileMapDisplay()
        {
            return (_value & 0x08) == 0 ? 0x9800 : 0x9c00;
        }

        public int GetBgWindowTileData()
        {
            return (_value & 0x10) == 0 ? 0x9000 : 0x8000;
        }

        public bool IsBgWindowTileDataSigned()
        {
            return (_value & 0x10) == 0;
        }

        public bool IsWindowDisplay()
        {
            return (_value & 0x20) != 0;
        }

        public int GetWindowTileMapDisplay()
        {
            return (_value & 0x40) == 0 ? 0x9800 : 0x9c00;
        }

        public bool IsLcdEnabled()
        {
            return (_value & 0x80) != 0;
        }

        public bool Accepts(int address)
        {
            return address == 0xff40;
        }

        public void SetByte(int address, int value)
        {
            // checkArgument(address == 0xff40);
            _value = value;
        }

        public int GetByte(int address)
        {
            // checkArgument(address == 0xff40);
            return _value;
        }

        public void Set(int value)
        {
            _value = value;
        }

        public int Get()
        {
            return _value;
        }
    }
}
