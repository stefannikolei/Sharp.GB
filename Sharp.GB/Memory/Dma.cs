﻿using Sharp.GB.Cpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory
{
    public class Dma : IAddressSpace
    {
        private readonly IAddressSpace _addressSpace;

        private readonly IAddressSpace _oam;

        private readonly SpeedMode _speedMode;

        private bool _transferInProgress;

        private bool _restarted;

        private int _from;

        private int _ticks;

        private int _regValue = 0xff;

        public Dma(IAddressSpace addressSpace, IAddressSpace oam, SpeedMode speedMode)
        {
            _addressSpace = new DmaAddressSpace(addressSpace);
            _speedMode = speedMode;
            _oam = oam;
        }

        public bool Accepts(int address)
        {
            return address == 0xff46;
        }

        public void Tick()
        {
            if (_transferInProgress)
            {
                if (++_ticks >= 648 / _speedMode.GetSpeedMode())
                {
                    _transferInProgress = false;
                    _restarted = false;
                    _ticks = 0;
                    for (int i = 0; i < 0xa0; i++)
                    {
                        _oam.SetByte(0xfe00 + i, _addressSpace.GetByte(_from + i));
                    }
                }
            }
        }


        public void SetByte(int address, int value)
        {
            _from = value * 0x100;
            _restarted = IsOamBlocked();
            _ticks = 0;
            _transferInProgress = true;
            _regValue = value;
        }


        public int GetByte(int address)
        {
            return _regValue;
        }

        public bool IsOamBlocked()
        {
            return _restarted || (_transferInProgress && _ticks >= 5);
        }
    }
}
