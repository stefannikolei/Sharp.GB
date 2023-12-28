using System;
using System.Text;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Gpu
{
    public class ColorPalette : IAddressSpace
    {
        private readonly int _indexAddr;

        private readonly int _dataAddr;

        private int[][] _palettes =
        [
            new int[4],
            new int[4],
            new int[4],
            new int[4],
            new int[4],
            new int[4],
            new int[4],
            new int[4]
        ];

        private int _index;

        private bool _autoIncrement;

        public ColorPalette(int offset)
        {
            _indexAddr = offset;
            _dataAddr = offset + 1;
        }

        public bool Accepts(int address)
        {
            return address == _indexAddr || address == _dataAddr;
        }

        public void SetByte(int address, int value)
        {
            if (address == _indexAddr)
            {
                _index = value & 0x3f;
                _autoIncrement = (value & (1 << 7)) != 0;
            }
            else if (address == _dataAddr)
            {
                int color = _palettes[_index / 8][(_index % 8) / 2];
                if (_index % 2 == 0)
                {
                    color = (color & 0xff00) | value;
                }
                else
                {
                    color = (color & 0x00ff) | (value << 8);
                }

                _palettes[_index / 8][(_index % 8) / 2] = color;
                if (_autoIncrement)
                {
                    _index = (_index + 1) & 0x3f;
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public int GetByte(int address)
        {
            if (address == _indexAddr)
            {
                return _index | (_autoIncrement ? 0x80 : 0x00) | 0x40;
            }
            else if (address == _dataAddr)
            {
                int color = _palettes[_index / 8][(_index % 8) / 2];
                if (_index % 2 == 0)
                {
                    return color & 0xff;
                }
                else
                {
                    return (color >> 8) & 0xff;
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public int[] GetPalette(int index)
        {
            return _palettes[index];
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                b.Append(i).Append(": ");
                int[] palette = GetPalette(i);
                foreach (int c in palette)
                {
                    b.Append(string.Format("%04X", c)).Append(' ');
                }

                b.Insert(b.Length - 1, '\n');
            }

            return b.ToString();
        }

        public void FillWithFf()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _palettes[i][j] = 0x7fff;
                }
            }
        }
    }
}
