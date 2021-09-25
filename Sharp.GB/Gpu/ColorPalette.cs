using System;
using System.Text;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Gpu
{
    public class ColorPalette : IAddressSpace
    {
        private readonly int indexAddr;

        private readonly int dataAddr;

        private int[][] palettes = {
            new int[4], new int[4], new int[4], new int[4], new int[4], new int[4], new int[4], new int[4]
        };

        private int index;

        private bool autoIncrement;

        public ColorPalette(int offset)
        {
            this.indexAddr = offset;
            this.dataAddr = offset + 1;
        }

        public bool accepts(int address)
        {
            return address == indexAddr || address == dataAddr;
        }

        public void setByte(int address, int value)
        {
            if (address == indexAddr)
            {
                index = value & 0x3f;
                autoIncrement = (value & (1 << 7)) != 0;
            }
            else if (address == dataAddr)
            {
                int color = palettes[index / 8][(index % 8) / 2];
                if (index % 2 == 0)
                {
                    color = (color & 0xff00) | value;
                }
                else
                {
                    color = (color & 0x00ff) | (value << 8);
                }

                palettes[index / 8][(index % 8) / 2] = color;
                if (autoIncrement)
                {
                    index = (index + 1) & 0x3f;
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }


        public int getByte(int address)
        {
            if (address == indexAddr)
            {
                return index | (autoIncrement ? 0x80 : 0x00) | 0x40;
            }
            else if (address == dataAddr)
            {
                int color = palettes[index / 8][(index % 8) / 2];
                if (index % 2 == 0)
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

        public int[] getPalette(int index)
        {
            return palettes[index];
        }


        public string toString()
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                b.Append(i).Append(": ");
                int[] palette = getPalette(i);
                foreach (int c in palette)
                {
                    b.Append(String.Format("%04X", c)).Append(' ');
                }

                b.Insert(b.Length - 1, '\n');
            }

            return b.ToString();
        }

        public void fillWithFF()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    palettes[i][j] = 0x7fff;
                }
            }
        }
    }
}
