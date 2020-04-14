namespace Sharp.GB.Memory
{
    public class Mmu : IAddressSpace
    {
        private readonly IAddressSpace _bootRom = new Rom(BootRom.GAMEBOY_CLASSIC, 0);
        private readonly IAddressSpace _ram = new Ram();

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
            if (address >= 0x00 && address <= 0xff)
            {
                return _bootRom;
            }
            else
            {
                return _ram;
            }
        }
    }
}