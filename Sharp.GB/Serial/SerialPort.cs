using System;
using Sharp.GB;
using Sharp.GB.Cpu;
using Sharp.GB.Memory.Interface;
using Sharp.GB.Serial;

public class SerialPort : IAddressSpace
{
    private readonly ISerialEndpoint _serialEndpoint;

    private readonly InterruptManager _interruptManager;

    private readonly bool _gbc;

    private int _sb;

    private int _sc;

    private bool _transferInProgress;

    private int _divider;

    private ClockType _clockType;

    private int _speed;

    private int _receivedBits;

    public SerialPort(InterruptManager interruptManager, ISerialEndpoint serialEndpoint, bool gbc)
    {
        _interruptManager = interruptManager;
        _serialEndpoint = serialEndpoint;
        _gbc = gbc;
    }

    public void Tick()
    {
        if (!_transferInProgress)
        {
            return;
        }

        var bitToTransfer = (_sb & (1 << 7)) != 0 ? 1 : 0;
        var incomingBit = -1;
        if (_clockType == ClockType.External)
        {
            incomingBit = _serialEndpoint.Receive(bitToTransfer);
        }
        else
        {
            if (_divider++ == Gameboy.TicksPerSec / _speed)
            {
                _divider = 0;
                incomingBit = _serialEndpoint.Send(bitToTransfer);
            }
        }

        if (incomingBit != -1)
        {
            _sb = (_sb << 1) & 0xff | (incomingBit & 1);
            _receivedBits++;
            if (_receivedBits == 8)
            {
                _interruptManager.RequestInterrupt(InterruptType.Serial);
                _transferInProgress = false;
            }
        }
    }

    public bool Accepts(int address)
    {
        return address == 0xff01 || address == 0xff02;
    }

    public void SetByte(int address, int value)
    {
        if (address == 0xff01)
        {
            _sb = value;
        }
        else if (address == 0xff02)
        {
            _sc = value;
            if ((_sc & (1 << 7)) != 0)
            {
                StartTransfer();
            }
        }
    }

    public int GetByte(int address)
    {
        if (address == 0xff01)
        {
            return _sb;
        }
        else if (address == 0xff02)
        {
            return _sc | 0b01111110;
        }
        else
        {
            throw new ArgumentException();
        }
    }

    private void StartTransfer()
    {
        _transferInProgress = true;
        _divider = 0;
        _clockType = GetFromSc(_sc);
        _receivedBits = 0;
        if (_gbc)
        {
            if ((_sc & (1 << 1)) == 0)
            {
                _speed = 8192;
            }
            else
            {
                _speed = 262144;
            }
        }
        else
        {
            _speed = 8192;
        }
    }

    public ClockType GetFromSc(int sc)
    {
        if ((sc & 1) == 1)
        {
            return ClockType.Internal;
        }
        else
        {
            return ClockType.External;
        }
    }
}

public enum ClockType
{
    Internal,
    External
}
