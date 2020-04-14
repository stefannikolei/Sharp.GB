namespace Sharp.GB.Memory
{
    public interface IAddressSpace
    {
        void SetByte(int address, int value);
        int GetByte(int address);
    }
}