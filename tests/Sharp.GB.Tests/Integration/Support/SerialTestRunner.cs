using System;
using System.IO;
using System.Text;
using Sharp.GB;
using Sharp.GB.Common;
using Sharp.GB.Controller;
using Sharp.GB.Cpu;
using Sharp.GB.Memory.cart;
using Sharp.GB.Serial;
using Sharp.GB.Sound;

public class SerialTestRunner : ByteReceiver
{
    private readonly Gameboy _gb;
    private readonly StringBuilder _text;
    private readonly TextWriter _os;

    public SerialTestRunner(FileInfo romFile, TextWriter os)
    {
        var options = new GameboyOptions(romFile.FullName);
        var cart = new Cartridge(options);
        _gb = new Gameboy(
            options,
            cart,
            NullDisplay.Instance,
            NullController.Instance,
            NullSoundOutput.Instance,
            new ByteReceivingSerialEndpoint(this)
        );
        _text = new StringBuilder();
        _os = os;
    }

    public string RunTest()
    {
        int divider = 0;
        while (true)
        {
            _gb.Tick();
            if (++divider == 4)
            {
                if (IsInfiniteLoop(_gb))
                {
                    break;
                }
                divider = 0;
            }
        }

        return _text.ToString();
    }

    public int Transfer(int outgoing)
    {
        try
        {
            _text.Append((char)outgoing);
            _os.WriteLine(outgoing);
            _os.Flush();
        }
        catch (IOException e)
        {
            // Handle exception
        }

        return 0;
    }

    public static bool IsInfiniteLoop(Gameboy gb)
    {
        var cpu = gb.GetCpu();

        if (cpu.GetState() != Cpu.State.Opcode)
        {
            return false;
        }

        var regs = cpu.GetRegisters();
        var mem = gb.GetAddressSpace();
        int i = regs.GetPc();
        bool found = true;

        foreach (int v in new int[] { 0x18, 0xfe })
        {
            if (mem.GetByte(i++) != v)
            {
                found = false;
                break;
            }
        }

        if (found)
        {
            return true;
        }

        i = regs.GetPc();

        foreach (int v in new int[] { 0xc3, BitUtils.GetLsb(i), BitUtils.GetMsb(i) })
        {
            if (mem.GetByte(i++) != v)
            {
                return false;
            }
        }

        return true;
    }

    public void OnNewByte(int receivedByte)
    {
        _text.Append((char)receivedByte);
        try
        {
            _os.Write(receivedByte);
            _os.Flush();
        }
        catch (IOException e)
        {
            throw new ApplicationException();
        }
    }
}

public interface ByteReceiver
{
    void OnNewByte(int receivedByte);
}

public class ByteReceivingSerialEndpoint : ISerialEndpoint
{
    private readonly ByteReceiver _byteReceiver;
    private int _receivedBits;
    private int _currentByte;

    public ByteReceivingSerialEndpoint(ByteReceiver byteReceiver)
    {
        _byteReceiver = byteReceiver;
    }

    public int Receive(int bitToTransfer)
    {
        _receivedBits++;
        _currentByte = _currentByte << 1 | (bitToTransfer & 1);
        if (_receivedBits == 8)
        {
            _byteReceiver.OnNewByte(_currentByte);
            _currentByte = 0;
            _receivedBits = 0;
        }
        return 0;
    }

    public int Send(int bitToTransfer)
    {
        return Receive(bitToTransfer);
    }
}
