using System;
using Sharp.GB.Gpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory
{
    public class Hdma : IAddressSpace
    {
        private static readonly int s_hdma1 = 0xff51;
        private static readonly int s_hdma2 = 0xff52;
        private static readonly int s_hdma3 = 0xff53;
        private static readonly int s_hdma4 = 0xff54;
        private static readonly int s_hdma5 = 0xff55;

        private readonly IAddressSpace _addressSpace;
        private readonly Ram _hdma1234 = new(s_hdma1, 4);

        private Mode _gpuMode;
        private bool _transferInProgress;
        private bool _hblankTransfer;
        private bool _lcdEnabled;
        private int _length;
        private int _src;
        private int _dst;
        private int _tick;

        public Hdma(IAddressSpace addressSpace)
        {
            _addressSpace = addressSpace;
        }

        public bool Accepts(int address)
        {
            return address >= s_hdma1 && address <= s_hdma5;
        }

        public void Tick()
        {
            if (!IsTransferInProgress())
            {
                return;
            }

            if (++_tick < 0x20)
            {
                return;
            }

            for (int j = 0; j < 0x10; j++)
            {
                _addressSpace.SetByte(_dst + j, _addressSpace.GetByte(_src + j));
            }

            _src += 0x10;
            _dst += 0x10;
            if (_length-- == 0)
            {
                _transferInProgress = false;
                _length = 0x7f;
            }
            else if (_hblankTransfer)
            {
                _gpuMode = Mode.Undefined; // wait until next HBlank
            }
        }

        public void SetByte(int address, int value)
        {
            if (_hdma1234.Accepts(address))
            {
                _hdma1234.SetByte(address, value);
            }
            else if (address == s_hdma5)
            {
                if (_transferInProgress && (address & (1 << 7)) == 0)
                {
                    StopTransfer();
                }
                else
                {
                    StartTransfer(value);
                }
            }
        }

        public int GetByte(int address)
        {
            if (_hdma1234.Accepts(address))
            {
                return 0xff;
            }
            else if (address == s_hdma5)
            {
                return (_transferInProgress ? 0 : (1 << 7)) | _length;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void OnGpuUpdate(Mode newGpuMode)
        {
            _gpuMode = newGpuMode;
        }

        public void OnLcdSwitch(bool lcdEnabled)
        {
            _lcdEnabled = lcdEnabled;
        }

        public bool IsTransferInProgress()
        {
            if (!_transferInProgress)
            {
                return false;
            }
            else if (_hblankTransfer && (_gpuMode == Mode.HBlank || !_lcdEnabled))
            {
                return true;
            }
            else if (!_hblankTransfer)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void StartTransfer(int reg)
        {
            _hblankTransfer = (reg & (1 << 7)) != 0;
            _length = reg & 0x7f;

            _src = (_hdma1234.GetByte(s_hdma1) << 8) | (_hdma1234.GetByte(s_hdma2) & 0xf0);
            _dst = ((_hdma1234.GetByte(s_hdma3) & 0x1f) << 8) | (_hdma1234.GetByte(s_hdma4) & 0xf0);
            _src = _src & 0xfff0;
            _dst = (_dst & 0x1fff) | 0x8000;

            _transferInProgress = true;
        }

        private void StopTransfer()
        {
            _transferInProgress = false;
        }
    }
}
