using System;

namespace Sharp.GB.Sound;

public class SoundMode2 : AbstractSoundMode
{
    private int _freqDivider;

    private int _lastOutput;

    private int _i;

    private VolumeEnvelope _volumeEnvelope;

    public SoundMode2(bool gbc)
        : base(0xff15, 64, gbc)
    {
        _volumeEnvelope = new VolumeEnvelope();
    }

    public override void Start()
    {
        _i = 0;
        if (Gbc)
        {
            _length.Reset();
        }

        _length.Start();
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
}
