﻿using Sharp.GB.Cpu;
using Sharp.GB.Gpu.Phase;
using Sharp.GB.Memory;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Gpu
{
    public class Gpu
    {
        private readonly IAddressSpace videoRam0;

        private readonly IAddressSpace videoRam1;

        private readonly IAddressSpace oamRam;

        private readonly IDisplay display;

        private readonly InterruptManager interruptManager;

        private readonly Dma dma;

        private readonly Lcdc lcdc;

        private readonly bool gbc;

        private readonly ColorPalette bgPalette;

        private readonly ColorPalette oamPalette;

        private readonly HBlankPhase hBlankPhase;

        private readonly OamSearch oamSearchPhase;

        private readonly PixelTransfer pixelTransferPhase;

        private readonly VBlankPhase vBlankPhase;

        private bool lcdEnabled = true;

        private int lcdEnabledDelay;

        private MemoryRegisters r;

        private int ticksInLine;

        private Mode mode;

        private IGpuPhase phase;

        public Gpu(IDisplay display, InterruptManager interruptManager, Dma dma, Ram oamRam, bool gbc)
        {
            this.r = new MemoryRegisters(GpuRegister.GetAll());
            this.lcdc = new Lcdc();
            this.interruptManager = interruptManager;
            this.gbc = gbc;
            this.videoRam0 = new Ram(0x8000, 0x2000);
            if (gbc)
            {
                this.videoRam1 = new Ram(0x8000, 0x2000);
            }
            else
            {
                this.videoRam1 = null;
            }

            this.oamRam = oamRam;
            this.dma = dma;

            this.bgPalette = new ColorPalette(0xff68);
            this.oamPalette = new ColorPalette(0xff6a);
            oamPalette.fillWithFF();

            this.oamSearchPhase = new OamSearch(oamRam, lcdc, r);
            this.pixelTransferPhase =
                new PixelTransfer(videoRam0, videoRam1, oamRam, display, lcdc, r, gbc, bgPalette, oamPalette);
            this.hBlankPhase = new HBlankPhase();
            this.vBlankPhase = new VBlankPhase();

            this.mode = Mode.OamSearch;
            this.phase = oamSearchPhase.start();

            this.display = display;
        }

        private IAddressSpace getAddressSpace(int address)
        {
            if (videoRam0.accepts(address) /* && mode != Mode.PixelTransfer*/)
            {
                return getVideoRam();
            }
            else if (
                oamRam.accepts(address) &&
                !dma.IsOamBlocked() /* && mode != Mode.OamSearch && mode != Mode.PixelTransfer*/)
            {
                return oamRam;
            }
            else if (lcdc.accepts(address))
            {
                return lcdc;
            }
            else if (r.accepts(address))
            {
                return r;
            }
            else if (gbc && bgPalette.accepts(address))
            {
                return bgPalette;
            }
            else if (gbc && oamPalette.accepts(address))
            {
                return oamPalette;
            }
            else
            {
                return null;
            }
        }

        private IAddressSpace getVideoRam()
        {
            if (gbc && (r[GpuRegister.VBK()] & 1) == 1)
            {
                return videoRam1;
            }
            else
            {
                return videoRam0;
            }
        }

        public IAddressSpace getVideoRam0()
        {
            return videoRam0;
        }

        public IAddressSpace getVideoRam1()
        {
            return videoRam1;
        }

        public bool accepts(int address)
        {
            return getAddressSpace(address) != null;
        }

        public void setByte(int address, int value)
        {
            if (address == GpuRegister.STAT().GetAddress())
            {
                setStat(value);
            }
            else
            {
                IAddressSpace space = getAddressSpace(address);
                if (space == lcdc)
                {
                    setLcdc(value);
                }
                else if (space != null)
                {
                    space.setByte(address, value);
                }
            }
        }

        public int getByte(int address)
        {
            if (address == GpuRegister.STAT().GetAddress())
            {
                return getStat();
            }
            else
            {
                IAddressSpace space = getAddressSpace(address);
                if (space == null)
                {
                    return 0xff;
                }
                else if (address == GpuRegister.VBK().GetAddress())
                {
                    return gbc ? 0xfe : 0xff;
                }
                else
                {
                    return space.getByte(address);
                }
            }
        }

        public Mode tick()
        {
            if (!lcdEnabled)
            {
                if (lcdEnabledDelay != -1)
                {
                    if (--lcdEnabledDelay == 0)
                    {
                        display.enableLcd();
                        lcdEnabled = true;
                    }
                }
            }

            if (!lcdEnabled)
            {
                return Mode.UNDEFINED;
            }

            Mode oldMode = mode;
            ticksInLine++;
            if (phase.tick())
            {
                // switch line 153 to 0
                if (ticksInLine == 4 && mode == Mode.VBlank && r[GpuRegister.LY()] == 153)
                {
                    r.put(GpuRegister.LY(), 0);
                    requestLycEqualsLyInterrupt();
                }
            }
            else
            {
                switch (oldMode)
                {
                    case Mode.OamSearch:
                        mode = Mode.PixelTransfer;
                        phase = pixelTransferPhase.start(oamSearchPhase.getSprites());
                        break;

                    case Mode.PixelTransfer:
                        mode = Mode.HBlank;
                        phase = hBlankPhase.start(ticksInLine);
                        requestLcdcInterrupt(3);
                        break;

                    case Mode.HBlank:
                        ticksInLine = 0;
                        if (r.PreIncrement(GpuRegister.LY()) == 144)
                        {
                            mode = Mode.VBlank;
                            phase = vBlankPhase.start();
                            interruptManager.requestInterrupt(InterruptType.VBlank());
                            requestLcdcInterrupt(4);
                        }
                        else
                        {
                            mode = Mode.OamSearch;
                            phase = oamSearchPhase.start();
                        }

                        requestLcdcInterrupt(5);
                        requestLycEqualsLyInterrupt();
                        break;

                    case Mode.VBlank:
                        ticksInLine = 0;
                        if (r.PreIncrement(GpuRegister.LY()) == 1)
                        {
                            mode = Mode.OamSearch;
                            r.put(GpuRegister.LY(), 0);
                            phase = oamSearchPhase.start();
                            requestLcdcInterrupt(5);
                        }
                        else
                        {
                            phase = vBlankPhase.start();
                        }

                        requestLycEqualsLyInterrupt();
                        break;
                }
            }

            if (oldMode == mode)
            {
                return Mode.UNDEFINED;
            }
            else
            {
                return mode;
            }
        }

        public int getTicksInLine()
        {
            return ticksInLine;
        }

        private void requestLcdcInterrupt(int statBit)
        {
            if ((r[GpuRegister.STAT()] & (1 << statBit)) != 0)
            {
                interruptManager.requestInterrupt(InterruptType.LCDC);
            }
        }

        private void requestLycEqualsLyInterrupt()
        {
            if (r[GpuRegister.LYC()] == r[GpuRegister.LY()])
            {
                requestLcdcInterrupt(6);
            }
        }

        private int getStat()
        {
            return r[GpuRegister.STAT()] | (int)mode | (r[GpuRegister.LYC()] == r[GpuRegister.LY()] ? (1 << 2) : 0) |
                   0x80;
        }

        private void setStat(int value)
        {
            r.put(GpuRegister.STAT(), value & 0b11111000); // last three bits are read-only
        }

        private void setLcdc(int value)
        {
            lcdc.set(value);
            if ((value & (1 << 7)) == 0)
            {
                disableLcd();
            }
            else
            {
                enableLcd();
            }
        }

        private void disableLcd()
        {
            r.put(GpuRegister.LY(), 0);
            this.ticksInLine = 0;
            this.phase = hBlankPhase.start(250);
            this.mode = Mode.HBlank;
            this.lcdEnabled = false;
            this.lcdEnabledDelay = -1;
            display.disableLcd();
        }

        private void enableLcd()
        {
            lcdEnabledDelay = 244;
        }

        public bool isLcdEnabled()
        {
            return lcdEnabled;
        }

        public Lcdc getLcdc()
        {
            return lcdc;
        }

        public MemoryRegisters getRegisters()
        {
            return r;
        }

        public bool isGbc()
        {
            return gbc;
        }

        public ColorPalette getBgPalette()
        {
            return bgPalette;
        }

        public Mode getMode()
        {
            return mode;
        }
    }
}
