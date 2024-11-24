using System;
using System.Collections.Generic;
using System.IO;
using Sharp.GB;
using Sharp.GB.Common;
using Sharp.GB.Controller;
using Sharp.GB.Cpu;
using Sharp.GB.Memory.cart;
using Sharp.GB.Memory.Interface;
using Sharp.GB.Serial;
using Sharp.GB.Sound;
using Xunit.Abstractions;

public class MooneyeTestRunner
{
    private Gameboy _gb;
    private Cpu _cpu;
    private IAddressSpace _mem;
    private Registers _regs;
    private ITestOutputHelper _os;

    public MooneyeTestRunner(FileInfo romFile, ITestOutputHelper os)
    {
        List<string> opts = new List<string>();
        if (
            romFile.FullName.ToString().EndsWith("-C.gb")
            || romFile.FullName.ToString().EndsWith("-cgb.gb")
        )
        {
            opts.Add("c");
        }
        if (romFile.Name.StartsWith("boot_"))
        {
            opts.Add("b");
        }
        opts.Add("db");
        GameboyOptions options = new GameboyOptions(romFile.FullName, new List<string>(), opts);
        Cartridge cart = new Cartridge(options);
        _gb = new Gameboy(
            options,
            cart,
            NullDisplay.Instance,
            NullController.Instance,
            NullSoundOutput.Instance,
            NullEndpoint.Instance
        );
        Console.WriteLine("System type: " + (cart.IsGbc ? "CGB" : "DMG"));
        Console.WriteLine("Bootstrap: " + (options.UseBootstrap ? "enabled" : "disabled"));
        _cpu = _gb.GetCpu();
        _regs = _cpu.GetRegisters();
        _mem = _gb.GetAddressSpace();
        _os = os;
    }

    public bool RunTest()
    {
        int divider = 0;
        while (!IsByteSequenceAtPc([0x00, 0x18, 0xfd]))
        {
            _gb.Tick();
            if (++divider >= (_gb.GetSpeedMode().GetSpeedMode() == 2 ? 1 : 4))
            {
                DisplayProgress();
                divider = 0;
            }
        }
        return _regs.GetA() == 0
            && _regs.GetB() == 3
            && _regs.GetC() == 5
            && _regs.GetD() == 8
            && _regs.GetE() == 13
            && _regs.GetH() == 21
            && _regs.GetL() == 34;
    }

    private void DisplayProgress()
    {
        if (
            _cpu.GetState() == Cpu.State.Opcode
            && _mem.GetByte(_regs.GetPc()) == 0x22
            && _regs.GetHl() >= 0x9800
            && _regs.GetHl() < 0x9c00
        )
        {
            if (_regs.GetA() != 0)
            {
                _os.WriteLine(_regs.GetA().ToString());
            }
        }
        else if (IsByteSequenceAtPc([0x7d, 0xe6, 0x1f, 0xee, 0x1f]))
        {
            _os.WriteLine('\n'.ToString());
        }
    }

    private bool IsByteSequenceAtPc(int[] seq)
    {
        if (_cpu.GetState() != Cpu.State.Opcode)
        {
            return false;
        }

        int i = _regs.GetPc();
        bool found = true;
        foreach (int v in seq)
        {
            if (_mem.GetByte(i++) != v)
            {
                found = false;
                break;
            }
        }
        return found;
    }
}
