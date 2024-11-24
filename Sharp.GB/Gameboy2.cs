using System;
using System.Collections.Generic;
using System.Threading;
using Sharp.GB.Common;
using Sharp.GB.Controller;
using Sharp.GB.Cpu;
using Sharp.GB.Debug;
using Sharp.GB.Gpu;
using Sharp.GB.Memory;
using Sharp.GB.Memory.cart;
using Sharp.GB.Memory.Interface;
using Sharp.GB.Serial;
using Sharp.GB.Sound;

public class Gameboy
{
    public static readonly int TicksPerSec = 4_194_304;

    private readonly Gpu _gpu;

    private readonly Mmu _mmu;

    private readonly Cpu _cpu;

    private readonly Sharp.GB.Timer _timer;

    private readonly Dma _dma;

    private readonly Hdma _hdma;

    private readonly IDisplay _display;

    private readonly Sound _sound;

    private readonly SerialPort _serialPort;

    private readonly bool _gbc;

    private readonly SpeedMode _speedMode;

    private readonly GameboyConsole? _console;

    private volatile bool _doStop;

    private readonly List<Thread> _tickListeners = new();

    private bool _requestedScreenRefresh;

    private bool _lcdDisabled;

    public Gameboy(
        GameboyOptions options,
        Cartridge rom,
        IDisplay display,
        IController controller,
        ISoundOutput soundOutput,
        ISerialEndpoint serialEndpoint
    )
        : this(options, rom, display, controller, soundOutput, serialEndpoint, null) { }

    public Gameboy(
        GameboyOptions options,
        Cartridge rom,
        IDisplay display,
        IController controller,
        ISoundOutput soundOutput,
        ISerialEndpoint serialEndpoint,
        GameboyConsole? console
    )
    {
        this._display = display;
        _gbc = rom.IsGbc;
        _speedMode = new();
        InterruptManager interruptManager = new InterruptManager(_gbc);
        _timer = new(interruptManager, _speedMode);
        _mmu = new();

        Ram oamRam = new Ram(0xfe00, 0x00a0);
        _dma = new(_mmu, oamRam, _speedMode);
        _gpu = new(display, interruptManager, _dma, oamRam, _gbc);
        _hdma = new(_mmu);
        _sound = new(soundOutput, _gbc);
        _serialPort = new(interruptManager, serialEndpoint, _gbc);
        _mmu.AddAddressSpace(rom);
        _mmu.AddAddressSpace(_gpu);
        _mmu.AddAddressSpace(new Joypad(interruptManager, controller));
        _mmu.AddAddressSpace(interruptManager);
        _mmu.AddAddressSpace(_serialPort);
        _mmu.AddAddressSpace(_timer);
        _mmu.AddAddressSpace(_dma);
        _mmu.AddAddressSpace(_sound);

        _mmu.AddAddressSpace(new Ram(0xc000, 0x1000));
        if (_gbc)
        {
            _mmu.AddAddressSpace(_speedMode);
            _mmu.AddAddressSpace(_hdma);
            _mmu.AddAddressSpace(new GbcRam());
            _mmu.AddAddressSpace(new UndocumentedGbcRegisters());
        }
        else
        {
            _mmu.AddAddressSpace(new Ram(0xd000, 0x1000));
        }
        _mmu.AddAddressSpace(new Ram(0xff80, 0x7f));
        _mmu.AddAddressSpace(new ShadowAddressSpace(_mmu, 0xe000, 0xc000, 0x1e00));
        _mmu.IndexSpaces();

        _cpu = new(_mmu, interruptManager, _gpu, display, _speedMode);

        interruptManager.DisableInterrupts(false);
        if (!options.UseBootstrap)
        {
            InitRegs();
        }

        this._console = console;
    }

    private void InitRegs()
    {
        Registers r = _cpu.GetRegisters();

        r.SetAf(0x01b0);
        if (_gbc)
        {
            r.SetA(0x11);
        }
        r.SetBc(0x0013);
        r.SetDe(0x00d8);
        r.SetHl(0x014d);
        r.SetSp(0xfffe);
        r.SetPc(0x0100);
    }

    public void Run()
    {
        _doStop = false;
        while (!_doStop)
        {
            Tick();
        }
    }

    public void Stop()
    {
        _doStop = true;
    }

    public void Tick()
    {
        Mode newMode = TickSubsystems();
        if (newMode != Mode.Undefined)
        {
            _hdma.OnGpuUpdate(newMode);
        }

        if (!_lcdDisabled && !_gpu.IsLcdEnabled())
        {
            _lcdDisabled = true;
            _display.FrameIsReady();
            _hdma.OnLcdSwitch(false);
        }
        else if (newMode == Mode.VBlank)
        {
            _requestedScreenRefresh = true;
            _display.FrameIsReady();
        }

        if (_lcdDisabled && _gpu.IsLcdEnabled())
        {
            _lcdDisabled = false;
            _hdma.OnLcdSwitch(true);
        }
        else if (_requestedScreenRefresh && newMode == Mode.OamSearch)
        {
            _requestedScreenRefresh = false;
        }
        if (_console != null)
        {
            _console.Tick();
        }

        foreach (var thread in _tickListeners)
        {
            thread.Start();
        }
    }

    private Mode TickSubsystems()
    {
        _timer.Tick();
        if (_hdma.IsTransferInProgress())
        {
            _hdma.Tick();
        }
        else
        {
            _cpu.Tick();
        }
        _dma.Tick();
        _sound.Tick();
        _serialPort.Tick();
        return _gpu.Tick();
    }

    public IAddressSpace GetAddressSpace()
    {
        return _mmu;
    }

    public Cpu GetCpu()
    {
        return _cpu;
    }

    public SpeedMode GetSpeedMode()
    {
        return _speedMode;
    }

    public Gpu GetGpu()
    {
        return _gpu;
    }

    public void RegisterTickListener(Thread tickListener)
    {
        _tickListeners.Add(tickListener);
    }

    public void UnregisterTickListener(Thread tickListener)
    {
        _tickListeners.Remove(tickListener);
    }

    public Sound GetSound()
    {
        return _sound;
    }
}
