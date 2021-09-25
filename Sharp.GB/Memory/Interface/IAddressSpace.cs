namespace Sharp.GB.Memory.Interface
{
    public interface IAddressSpace
    {
        bool accepts(int address);
        void setByte(int address, int value);
        int getByte(int address);
    }
}
