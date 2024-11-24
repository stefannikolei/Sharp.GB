namespace Sharp.GB.Memory.Interface
{
    public interface IAddressSpace
    {
        bool Accepts(int address);
        void SetByte(int address, int value);
        int GetByte(int address);
    }

    public class VoidAddressspace : IAddressSpace
    {
        public static IAddressSpace Instance => new VoidAddressspace();

        public bool Accepts(int address)
        {
            return true;
        }

        public void SetByte(int address, int value)
        {
            if (address < 0 || address > 0xffff)
            {
                throw new ArgumentException("Invalid address: " + string.Format("#", address));
            }
        }

        public int GetByte(int address)
        {
            if (address < 0 || address > 0xffff)
            {
                throw new ArgumentException("Invalid address: " + string.Format("#", address));
            }

            return 0xff;
        }
    }
}
