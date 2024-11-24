using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory
{
    public class Mmu : IAddressSpace
    {
        private readonly List<IAddressSpace> _spaces = new();

        private IAddressSpace[]? _addressToSpace;

        public void AddAddressSpace(IAddressSpace space)
        {
            _spaces.Add(space);
        }

        public void IndexSpaces()
        {
            _addressToSpace = new IAddressSpace[0x10000];
            for (int i = 0; i < _addressToSpace.Length; i++)
            {
                _addressToSpace[i] = VoidAddressspace.Instance;
                foreach (IAddressSpace s in _spaces)
                {
                    if (s.Accepts(i))
                    {
                        _addressToSpace[i] = s;
                        break;
                    }
                }
            }
        }

        public bool Accepts(int address)
        {
            return true;
        }

        public void SetByte(int address, int value)
        {
            GetSpace(address).SetByte(address, value);
        }

        public int GetByte(int address)
        {
            return GetSpace(address).GetByte(address);
        }

        private IAddressSpace GetSpace(int address)
        {
            if (_addressToSpace == null)
            {
                throw new ArgumentException("Address spaces hasn't been indexed yet");
            }
            return _addressToSpace[address];
        }
    }
}
