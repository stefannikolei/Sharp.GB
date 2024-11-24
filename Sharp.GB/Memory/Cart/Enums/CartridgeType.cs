namespace Sharp.GB.Memory.cart
{
    public enum CartridgeType
    {
        Rom = 0x00,
        RomMbc1 = 0x01,
        RomMbc1Ram = 0x02,
        RomMbc1RamBattery = 0x03,
        RomMbc2 = 0x05,
        RomMbc2Battery = 0x06,
        RomRam = 0x08,
        RomRamBattery = 0x09,
        RomMmm01 = 0x0b,
        RomMmm01Sram = 0x0c,
        RomMmm01SramBattery = 0x0d,
        RomMbc3TimerBattery = 0x0f,
        RomMbc3TimerRamBattery = 0x10,
        RomMbc3 = 0x11,
        RomMbc3Ram = 0x12,
        RomMbc3RamBattery = 0x13,
        RomMbc5 = 0x19,
        RomMbc5Ram = 0x1a,
        RomMbc5RamBattery = 0x01b,
        RomMbc5Rumble = 0x1c,
        RomMbc5RumbleSram = 0x1d,
        RomMbc5RumbleSramBattery = 0x1e,
    }
}
