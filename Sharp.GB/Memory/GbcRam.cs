using System;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory
{
    public class GbcRam : IAddressSpace
    {
        private int[] _ram = new int[7 * 0x1000];

        private int _svbk;

        public bool Accepts(int address)
        {
            return address == 0xff70 || (address >= 0xd000 && address < 0xe000);
        }


        public void SetByte(int address, int value)
        {
            if (address == 0xff70)
            {
                _svbk = value;
            }
            else
            {
                _ram[Translate(address)] = value;
            }
        }


        public int GetByte(int address)
        {
            if (address == 0xff70)
            {
                return _svbk;
            }
            else
            {
                return _ram[Translate(address)];
            }
        }

        private int Translate(int address)
        {
            int ramBank = _svbk & 0x7;
            if (ramBank == 0)
            {
                ramBank = 1;
            }

            int result = address - 0xd000 + (ramBank - 1) * 0x1000;
            if (result < 0 || result >= _ram.Length)
            {
                throw new ArgumentException();
            }

            return result;
        }
    }
}
