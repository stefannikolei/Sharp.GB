using System;
using Sharp.GB.Gpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory
{
    public class Hdma : IAddressSpace
    {
        private static readonly int HDMA1 = 0xff51;
        private static readonly int HDMA2 = 0xff52;
        private static readonly int HDMA3 = 0xff53;
        private static readonly int HDMA4 = 0xff54;
        private static readonly int HDMA5 = 0xff55;
        
        private readonly IAddressSpace addressSpace;
        private readonly Ram hdma1234 = new Ram(HDMA1, 4);
        
        private Gpu.Mode gpuMode;
        private bool transferInProgress;
        private bool hblankTransfer;
        private bool lcdEnabled;
        private int length;
        private int src;
        private int dst;
        private int tick;

        public Hdma(IAddressSpace addressSpace)
        {
            this.addressSpace = addressSpace;
        }

        public bool accepts(int address)
        {
            return address >= HDMA1 && address <= HDMA5;
        }

        public void Tick()
        {
            if (!IsTransferInProgress())
            {
                return;
            }

            if (++tick < 0x20)
            {
                return;
            }

            for (int j = 0; j < 0x10; j++)
            {
                addressSpace.setByte(dst + j, addressSpace.getByte(src + j));
            }

            src += 0x10;
            dst += 0x10;
            if (length-- == 0)
            {
                transferInProgress = false;
                length = 0x7f;
            }
            else if (hblankTransfer)
            {
                gpuMode = Mode.UNDEFINED; // wait until next HBlank
            }
        }

        public void setByte(int address, int value)
        {
            if (hdma1234.accepts(address))
            {
                hdma1234.setByte(address, value);
            }
            else if (address == HDMA5)
            {
                if (transferInProgress && (address & (1 << 7)) == 0)
                {
                    StopTransfer();
                }
                else
                {
                    StartTransfer(value);
                }
            }
        }

        public int getByte(int address)
        {
            if (hdma1234.accepts(address))
            {
                return 0xff;
            }
            else if (address == HDMA5)
            {
                return (transferInProgress ? 0 : (1 << 7)) | length;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void OnGpuUpdate(Gpu.Mode newGpuMode)
        {
            this.gpuMode = newGpuMode;
        }

        public void OnLcdSwitch(bool lcdEnabled)
        {
            this.lcdEnabled = lcdEnabled;
        }

        public bool IsTransferInProgress()
        {
            if (!transferInProgress)
            {
                return false;
            }
            else if (hblankTransfer && (gpuMode == Gpu.Mode.HBlank || !lcdEnabled))
            {
                return true;
            }
            else if (!hblankTransfer)
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
            hblankTransfer = (reg & (1 << 7)) != 0;
            length = reg & 0x7f;

            src = (hdma1234.getByte(HDMA1) << 8) | (hdma1234.getByte(HDMA2) & 0xf0);
            dst = ((hdma1234.getByte(HDMA3) & 0x1f) << 8) | (hdma1234.getByte(HDMA4) & 0xf0);
            src = src & 0xfff0;
            dst = (dst & 0x1fff) | 0x8000;

            transferInProgress = true;
        }

        private void StopTransfer()
        {
            transferInProgress = false;
        }
    }
}
