public class MemoryTestRunner
{
    private readonly Gameboy _gb;
    private readonly StringBuilder _text = new StringBuilder();
    private readonly TextWriter _os;
    private bool _testStarted = false;

    public MemoryTestRunner(FileInfo romFile, TextWriter os)
    {
        var options = new GameboyOptions(romFile.FullName);
        var cart = new Cartridge(options);
        _gb = new Gameboy(
            options,
            cart,
            NullDisplay.Instance,
            NullController.Instance,
            NullSoundOutput.Instance,
            NullEndpoint.Instance
        );
        _os = os;
    }

    public TestResult RunTest()
    {
        int status = 0x80;
        int divider = 0;
        while (status == 0x80 && !SerialTestRunner.IsInfiniteLoop(_gb))
        {
            _gb.Tick();
            if (++divider >= (_gb.GetSpeedMode().GetSpeedMode() == 2 ? 1 : 4))
            {
                status = GetTestResult(_gb);
                divider = 0;
            }
        }

        return new(status, _text.ToString());
    }

    private int GetTestResult(Gameboy gb)
    {
        var mem = gb.GetAddressSpace();

        int i;
        if (!_testStarted)
        {
            i = 0xa000;
            foreach (var v in new[] { 0x80, 0xde, 0xb0, 0x61 })
            {
                if (mem.GetByte(i++) != v)
                {
                    return 0x80;
                }
            }
            _testStarted = true;
        }

        int status = mem.GetByte(0xa000);
        if (gb.GetCpu().GetState() != Cpu.State.Opcode)
        {
            return status;
        }

        var reg = gb.GetCpu().GetRegisters();
        i = reg.GetPc();
        foreach (var v in new[] { 0xe5, 0xf5, 0xfa, 0x83, 0xd8 })
        {
            if (mem.GetByte(i++) != v)
            {
                return status;
            }
        }
        var c = (char)reg.GetA();
        _text.Append(c);
        if (_os != null)
        {
            _os.Write((byte)c);
        }
        reg.SetPc(reg.GetPc() + 0x19);
        return status;
    }

    public class TestResult
    {
        public int Status { get; }
        public string Text { get; }

        public TestResult(int status, string text)
        {
            Status = status;
            Text = text;
        }
    }
}
