using Sharp.GB.Cpu;
using Sharp.GB.Gpu.Phase;
using Sharp.GB.Memory;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Gpu
{
    public class Gpu : IAddressSpace
    {
        private readonly IAddressSpace _videoRam0;

        private readonly IAddressSpace? _videoRam1;

        private readonly IAddressSpace _oamRam;

        private readonly IDisplay _display;

        private readonly InterruptManager _interruptManager;

        private readonly Dma _dma;

        private readonly Lcdc _lcdc;

        private readonly bool _gbc;

        private readonly ColorPalette _bgPalette;

        private readonly ColorPalette _oamPalette;

        private readonly HBlankPhase _hBlankPhase;

        private readonly OamSearch _oamSearchPhase;

        private readonly PixelTransfer _pixelTransferPhase;

        private readonly VBlankPhase _vBlankPhase;

        private bool _lcdEnabled = true;

        private int _lcdEnabledDelay;

        private MemoryRegisters _r;

        private int _ticksInLine;

        private Mode _mode;

        private IGpuPhase _phase;

        public Gpu(
            IDisplay display,
            InterruptManager interruptManager,
            Dma dma,
            Ram oamRam,
            bool gbc
        )
        {
            _r = new MemoryRegisters(GpuRegister.GetAll());
            _lcdc = new Lcdc();
            _interruptManager = interruptManager;
            _gbc = gbc;
            _videoRam0 = new Ram(0x8000, 0x2000);
            if (gbc)
            {
                _videoRam1 = new Ram(0x8000, 0x2000);
            }
            else
            {
                _videoRam1 = null;
            }

            _oamRam = oamRam;
            _dma = dma;

            _bgPalette = new ColorPalette(0xff68);
            _oamPalette = new ColorPalette(0xff6a);
            _oamPalette.FillWithFf();

            _oamSearchPhase = new OamSearch(oamRam, _lcdc, _r);
            _pixelTransferPhase = new PixelTransfer(
                _videoRam0,
                _videoRam1,
                oamRam,
                display,
                _lcdc,
                _r,
                gbc,
                _bgPalette,
                _oamPalette
            );
            _hBlankPhase = new HBlankPhase();
            _vBlankPhase = new VBlankPhase();

            _mode = Mode.OamSearch;
            _phase = _oamSearchPhase.Start();

            _display = display;
        }

        private IAddressSpace? GetAddressSpace(int address)
        {
            if (
                _videoRam0.Accepts(address) /* && mode != Mode.PixelTransfer*/
            )
            {
                return GetVideoRam();
            }
            else if (
                _oamRam.Accepts(address) && !_dma.IsOamBlocked() /* && mode != Mode.OamSearch && mode != Mode.PixelTransfer*/
            )
            {
                return _oamRam;
            }
            else if (_lcdc.Accepts(address))
            {
                return _lcdc;
            }
            else if (_r.Accepts(address))
            {
                return _r;
            }
            else if (_gbc && _bgPalette.Accepts(address))
            {
                return _bgPalette;
            }
            else if (_gbc && _oamPalette.Accepts(address))
            {
                return _oamPalette;
            }
            else
            {
                return null;
            }
        }

        private IAddressSpace GetVideoRam()
        {
            if (_gbc && (_r[GpuRegister.Vbk] & 1) == 1)
            {
                return _videoRam1!;
            }
            else
            {
                return _videoRam0;
            }
        }

        public IAddressSpace GetVideoRam0()
        {
            return _videoRam0;
        }

        public IAddressSpace? GetVideoRam1()
        {
            return _videoRam1;
        }

        public bool Accepts(int address)
        {
            return GetAddressSpace(address) != null;
        }

        public void SetByte(int address, int value)
        {
            if (address == GpuRegister.Stat.GetAddress())
            {
                SetStat(value);
            }
            else
            {
                IAddressSpace? space = GetAddressSpace(address);
                if (space == _lcdc)
                {
                    SetLcdc(value);
                }
                else if (space != null)
                {
                    space.SetByte(address, value);
                }
            }
        }

        public int GetByte(int address)
        {
            if (address == GpuRegister.Stat.GetAddress())
            {
                return GetStat();
            }
            else
            {
                IAddressSpace? space = GetAddressSpace(address);
                if (space == null)
                {
                    return 0xff;
                }
                else if (address == GpuRegister.Vbk.GetAddress())
                {
                    return _gbc ? 0xfe : 0xff;
                }
                else
                {
                    return space.GetByte(address);
                }
            }
        }

        public Mode Tick()
        {
            if (!_lcdEnabled)
            {
                if (_lcdEnabledDelay != -1)
                {
                    if (--_lcdEnabledDelay == 0)
                    {
                        _display.EnableLcd();
                        _lcdEnabled = true;
                    }
                }
            }

            if (!_lcdEnabled)
            {
                return Mode.Undefined;
            }

            Mode oldMode = _mode;
            _ticksInLine++;
            if (_phase.Tick())
            {
                // switch line 153 to 0
                if (_ticksInLine == 4 && _mode == Mode.VBlank && _r[GpuRegister.Ly] == 153)
                {
                    _r.Put(GpuRegister.Ly, 0);
                    RequestLycEqualsLyInterrupt();
                }
            }
            else
            {
                switch (oldMode)
                {
                    case Mode.OamSearch:
                        _mode = Mode.PixelTransfer;
                        _phase = _pixelTransferPhase.Start(_oamSearchPhase.GetSprites());
                        break;

                    case Mode.PixelTransfer:
                        _mode = Mode.HBlank;
                        _phase = _hBlankPhase.Start(_ticksInLine);
                        RequestLcdcInterrupt(3);
                        break;

                    case Mode.HBlank:
                        _ticksInLine = 0;
                        if (_r.PreIncrement(GpuRegister.Ly) == 144)
                        {
                            _mode = Mode.VBlank;
                            _phase = _vBlankPhase.Start();
                            _interruptManager.RequestInterrupt(InterruptType.VBlank);
                            RequestLcdcInterrupt(4);
                        }
                        else
                        {
                            _mode = Mode.OamSearch;
                            _phase = _oamSearchPhase.Start();
                        }

                        RequestLcdcInterrupt(5);
                        RequestLycEqualsLyInterrupt();
                        break;

                    case Mode.VBlank:
                        _ticksInLine = 0;
                        if (_r.PreIncrement(GpuRegister.Ly) == 1)
                        {
                            _mode = Mode.OamSearch;
                            _r.Put(GpuRegister.Ly, 0);
                            _phase = _oamSearchPhase.Start();
                            RequestLcdcInterrupt(5);
                        }
                        else
                        {
                            _phase = _vBlankPhase.Start();
                        }

                        RequestLycEqualsLyInterrupt();
                        break;
                }
            }

            if (oldMode == _mode)
            {
                return Mode.Undefined;
            }
            else
            {
                return _mode;
            }
        }

        public int GetTicksInLine()
        {
            return _ticksInLine;
        }

        private void RequestLcdcInterrupt(int statBit)
        {
            if ((_r[GpuRegister.Stat] & (1 << statBit)) != 0)
            {
                _interruptManager.RequestInterrupt(InterruptType.Lcdc);
            }
        }

        private void RequestLycEqualsLyInterrupt()
        {
            if (_r[GpuRegister.Lyc] == _r[GpuRegister.Ly])
            {
                RequestLcdcInterrupt(6);
            }
        }

        private int GetStat()
        {
            return _r[GpuRegister.Stat]
                | (int)_mode
                | (_r[GpuRegister.Lyc] == _r[GpuRegister.Ly] ? (1 << 2) : 0)
                | 0x80;
        }

        private void SetStat(int value)
        {
            _r.Put(GpuRegister.Stat, value & 0b11111000); // last three bits are read-only
        }

        private void SetLcdc(int value)
        {
            _lcdc.Set(value);
            if ((value & (1 << 7)) == 0)
            {
                DisableLcd();
            }
            else
            {
                EnableLcd();
            }
        }

        private void DisableLcd()
        {
            _r.Put(GpuRegister.Ly, 0);
            _ticksInLine = 0;
            _phase = _hBlankPhase.Start(250);
            _mode = Mode.HBlank;
            _lcdEnabled = false;
            _lcdEnabledDelay = -1;
            _display.DisableLcd();
        }

        private void EnableLcd()
        {
            _lcdEnabledDelay = 244;
        }

        public bool IsLcdEnabled()
        {
            return _lcdEnabled;
        }

        public Lcdc GetLcdc()
        {
            return _lcdc;
        }

        public MemoryRegisters GetRegisters()
        {
            return _r;
        }

        public bool IsGbc()
        {
            return _gbc;
        }

        public ColorPalette GetBgPalette()
        {
            return _bgPalette;
        }

        public Mode GetMode()
        {
            return _mode;
        }
    }
}
