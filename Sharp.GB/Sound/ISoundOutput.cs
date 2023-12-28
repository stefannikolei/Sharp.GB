namespace Sharp.GB.Sound;

public interface ISoundOutput
{
    void Start();

    void Stop();

    void Play(int left, int right);
}

public class NullSoundOutput : ISoundOutput
{
    public static ISoundOutput Instance => new NullSoundOutput();

    public void Start() { }

    public void Stop() { }

    public void Play(int left, int right) { }
}
