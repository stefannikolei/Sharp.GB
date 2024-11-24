using System;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory
{
    public class Ram : IAddressSpace
    {
        private readonly int _offset;
        private readonly int _length;
        private readonly int[] _space;

        public Ram(int offset, int length)
        {
            _offset = offset;
            _length = length;
            _space = new int[length];
        }

        private Ram(int offset, int length, Ram ram)
        {
            _offset = offset;
            _length = length;
            _space = ram._space;
        }

        public static Ram CreateShadow(int offset, int length, Ram ram)
        {
            return new(offset, length, ram);
        }

        public bool Accepts(int address)
        {
            return address >= _offset && address < _offset + _length;
        }

        public void SetByte(int address, int value)
        {
            _space[address - _offset] = value;
        }

        public int GetByte(int address)
        {
            var index = address - _offset;

            if (index < 0 || index >= _space.Length)
            {
                throw new IndexOutOfRangeException("Address: " + address);
            }

            return _space[index];
        }
    }
}
