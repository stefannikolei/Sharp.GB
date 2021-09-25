using System;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory
{
    public class UndocumentedGbcRegisters : IAddressSpace
    {
        private readonly IAddressSpace _ram = new Ram(0xff72, 6);
        private int _xff6c = 0xfe;


        public UndocumentedGbcRegisters()
        {
            _ram.setByte(0xff74, 0xff);
            _ram.setByte(0xff75, 0x8f);
        }

        public bool accepts(int address)
        {
            return address == 0xff6c || _ram.accepts(address);
        }

        public void setByte(int address, int value)
        {
            switch (address)
            {
                case 0xff6c:
                    _xff6c = 0xfe | (value & 1);
                    break;

                case 0xff72:
                case 0xff73:
                case 0xff74:
                    _ram.setByte(address, value);
                    break;

                case 0xff75:
                    _ram.setByte(address, 0x8f | (value & 0b01110000));
                    break;
            }
        }

        public int getByte(int address)
        {
            if (address == 0xff6c)
            {
                return _xff6c;
            }

            if (_ram.accepts(address))
            {
                return _ram.getByte(address);
            }

            throw new ArgumentException();
        }
    }
}
