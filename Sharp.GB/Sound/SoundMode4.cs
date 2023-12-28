namespace Sharp.GB.Sound;

public class SoundMode4 : AbstractSoundMode
{
    private VolumeEnvelope _volumeEnvelope;

    private PolynomialCounter _polynomialCounter;

    private int _lastResult;

    private Lfsr _lfsr = new Lfsr();

    public SoundMode4(bool gbc)
        : base(0xff1f, 64, gbc)
    {
        _volumeEnvelope = new VolumeEnvelope();
        _polynomialCounter = new PolynomialCounter();
    }

    public override void Start()
    {
        if (Gbc)
        {
            _length.Reset();
        }

        _length.Start();
        _lfsr.Start();
        _volumeEnvelope.Start();
    }

    protected override void Trigger()
    {
        _lfsr.Reset();
        _volumeEnvelope.Trigger();
    }

    public override int Tick()
    {
        _volumeEnvelope.Tick();

        if (!UpdateLength())
        {
            return 0;
        }

        if (!_dacEnabled)
        {
            return 0;
        }

        if (_polynomialCounter.Tick())
        {
            _lastResult = _lfsr.NextBit((_nr3 & (1 << 3)) != 0);
        }

        return _lastResult * _volumeEnvelope.GetVolume();
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
        _polynomialCounter.SetNr43(value);
    }
}
