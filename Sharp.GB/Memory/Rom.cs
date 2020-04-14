namespace Sharp.GB.Memory
{
    public class Rom : IAddressSpace
    {
        private readonly int[] _space;
        private readonly int _offset;

        public Rom(int[] space, int offset)
        {
            _space = space;
            _offset = offset;
        }

        public void SetByte(int address, int value)
        {
        }

        public int GetByte(int address)
        {
            if (_offset > address)
            {
                return 0;
            }

            var position = address - _offset;
            if (position >= _space.Length)
            {
                return 0;
            }

            return _space[position];
        }
    }
}