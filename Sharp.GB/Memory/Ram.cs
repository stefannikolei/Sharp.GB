namespace Sharp.GB.Memory
{
    public class Ram : IAddressSpace
    {
        private readonly int[] _space = new int[0x10000];
        
        public void SetByte(int address, int value)
        {
            _space[address] = value;
        }

        public int GetByte(int address)
        {
            return _space[address];
        }
    }
}