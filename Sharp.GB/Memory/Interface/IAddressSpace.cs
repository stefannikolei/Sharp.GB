﻿namespace Sharp.GB.Memory.Interface
{
    public interface IAddressSpace
    {
        bool Accepts(int address);
        void SetByte(int address, int value);
        int GetByte(int address);
    }
}
