using System;

namespace Sharp.GB.Sound;

public class SoundMode1 : AbstractSoundMode
{
    private int _freqDivider;

    private int _lastOutput;

    private int _i;

    private FrequencySweep _frequencySweep;

    private VolumeEnvelope _volumeEnvelope;

    public SoundMode1(bool gbc)
        : base(0xff10, 64, gbc)
    {
        _frequencySweep = new();
        _volumeEnvelope = new();
    }

    public override void Start()
    {
        _i = 0;
        if (Gbc)
        {
            _length.Reset();
        }

        _length.Start();
        _frequencySweep.Start();
        _volumeEnvelope.Start();
    }

    protected override void Trigger()
    {
        _i = 0;
        _freqDivider = 1;
        _volumeEnvelope.Trigger();
    }

    public override int Tick()
    {
        _volumeEnvelope.Tick();

        bool e = true;
        e = UpdateLength() && e;
        e = UpdateSweep() && e;
        e = _dacEnabled && e;
        if (!e)
        {
            return 0;
        }

        if (--_freqDivider == 0)
        {
            ResetFreqDivider();
            _lastOutput = ((GetDuty() & (1 << _i)) >> _i);
            _i = (_i + 1) % 8;
        }

        return _lastOutput * _volumeEnvelope.GetVolume();
    }

    protected new void SetNr0(int value)
    {
        base.SetNr0(value);
        _frequencySweep.SetNr10(value);
    }

    protected new void SetNr1(int value)
    {
        base.SetNr1(value);
        _length.SetLength(64 - (value & 0b00111111));
    }

    protected new void SetNr2(int value)
    {
        base.SetNr2(value);
        _volumeEnvelope.SetNr2(value);
        _dacEnabled = (value & 0b11111000) != 0;
        _channelEnabled &= _dacEnabled;
    }

    protected new void SetNr3(int value)
    {
        base.SetNr3(value);
        _frequencySweep.SetNr13(value);
    }

    protected new void SetNr4(int value)
    {
        base.SetNr4(value);
        _frequencySweep.SetNr14(value);
    }

    protected new int GetNr3()
    {
        return _frequencySweep.GetNr13();
    }

    protected new int GetNr4()
    {
        return (base.GetNr4() & 0b11111000) | (_frequencySweep.GetNr14() & 0b00000111);
    }

    private int GetDuty()
    {
        switch (GetNr1() >> 6)
        {
            case 0:
                return 0b00000001;
            case 1:
                return 0b10000001;
            case 2:
                return 0b10000111;
            case 3:
                return 0b01111110;
            default:
                throw new ApplicationException();
        }
    }

    private void ResetFreqDivider()
    {
        _freqDivider = GetFrequency() * 4;
    }

    protected bool UpdateSweep()
    {
        _frequencySweep.Tick();
        if (_channelEnabled && !_frequencySweep.IsEnabled())
        {
            _channelEnabled = false;
        }

        return _channelEnabled;
    }
}
