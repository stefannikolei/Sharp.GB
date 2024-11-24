using System;
using Sharp.GB.Memory;

namespace Sharp.GB.Sound;

public class SoundMode3 : AbstractSoundMode
{
    private static readonly int[] s_dmgWave =
    [
        0x84, 0x40, 0x43, 0xaa, 0x2d, 0x78, 0x92, 0x3c, 0x60, 0x59, 0x59, 0xb0, 0x34, 0xb8, 0x2e, 0xda
    ];

    private static readonly int[] s_cgbWave =
    [
        0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff
    ];

    private readonly Ram _waveRam = new(0xff30, 0x10);

    private int _freqDivider;

    private int _lastOutput;

    private int _i;

    private int _ticksSinceRead = 65536;

    private int _lastReadAddr;

    private int _buffer;

    private bool _triggered;

    public SoundMode3(bool gbc)
        : base(0xff1a, 256, gbc)
    {
        foreach (int v in gbc ? s_cgbWave : s_dmgWave)
        {
            _waveRam.SetByte(0xff30, v);
        }
    }

    public new bool Accepts(int address)
    {
        return _waveRam.Accepts(address) || base.Accepts(address);
    }

    public new int GetByte(int address)
    {
        if (!_waveRam.Accepts(address))
        {
            return base.GetByte(address);
        }

        if (!IsEnabled())
        {
            return _waveRam.GetByte(address);
        }
        else if (_waveRam.Accepts(_lastReadAddr) && (Gbc || _ticksSinceRead < 2))
        {
            return _waveRam.GetByte(_lastReadAddr);
        }
        else
        {
            return 0xff;
        }
    }

    public new void SetByte(int address, int value)
    {
        if (!_waveRam.Accepts(address))
        {
            base.SetByte(address, value);
            return;
        }

        if (!IsEnabled())
        {
            _waveRam.SetByte(address, value);
        }
        else if (_waveRam.Accepts(_lastReadAddr) && (Gbc || _ticksSinceRead < 2))
        {
            _waveRam.SetByte(_lastReadAddr, value);
        }
    }

    protected new void SetNr0(int value)
    {
        base.SetNr0(value);
        _dacEnabled = (value & (1 << 7)) != 0;
        _channelEnabled &= _dacEnabled;
    }

    protected new void SetNr1(int value)
    {
        base.SetNr1(value);
        _length.SetLength(256 - value);
    }

    protected new void SetNr3(int value)
    {
        base.SetNr3(value);
    }

    public new void SetNr4(int value)
    {
        if (!Gbc && (value & (1 << 7)) != 0)
        {
            if (IsEnabled() && _freqDivider == 2)
            {
                int pos = _i / 2;
                if (pos < 4)
                {
                    _waveRam.SetByte(0xff30, _waveRam.GetByte(0xff30 + pos));
                }
                else
                {
                    pos = pos & ~3;
                    for (int j = 0; j < 4; j++)
                    {
                        _waveRam.SetByte(0xff30 + j, _waveRam.GetByte(0xff30 + ((pos + j) % 0x10)));
                    }
                }
            }
        }

        base.SetNr4(value);
    }

    public override void Start()
    {
        _i = 0;
        _buffer = 0;
        if (Gbc)
        {
            _length.Reset();
        }

        _length.Start();
    }

    protected override void Trigger()
    {
        _i = 0;
        _freqDivider = 6;
        _triggered = !Gbc;
        if (Gbc)
        {
            GetWaveEntry();
        }
    }

    public override int Tick()
    {
        _ticksSinceRead++;
        if (!UpdateLength())
        {
            return 0;
        }

        if (!_dacEnabled)
        {
            return 0;
        }

        if ((GetNr0() & (1 << 7)) == 0)
        {
            return 0;
        }

        if (--_freqDivider == 0)
        {
            ResetFreqDivider();
            if (_triggered)
            {
                _lastOutput = (_buffer >> 4) & 0x0f;
                _triggered = false;
            }
            else
            {
                _lastOutput = GetWaveEntry();
            }

            _i = (_i + 1) % 32;
        }

        return _lastOutput;
    }

    private int GetVolume()
    {
        return (GetNr2() >> 5) & 0b11;
    }

    private int GetWaveEntry()
    {
        _ticksSinceRead = 0;
        _lastReadAddr = 0xff30 + _i / 2;
        _buffer = _waveRam.GetByte(_lastReadAddr);
        int b = _buffer;
        if (_i % 2 == 0)
        {
            b = (b >> 4) & 0x0f;
        }
        else
        {
            b = b & 0x0f;
        }

        switch (GetVolume())
        {
            case 0:
                return 0;
            case 1:
                return b;
            case 2:
                return b >> 1;
            case 3:
                return b >> 2;
            default:
                throw new ApplicationException();
        }
    }

    private void ResetFreqDivider()
    {
        _freqDivider = GetFrequency() * 2;
    }
}
