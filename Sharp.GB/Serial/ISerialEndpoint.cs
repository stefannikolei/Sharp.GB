namespace Sharp.GB.Serial;

public interface ISerialEndpoint
{
    int Transfer(int outgoing);
}

public class NullEndpoint : ISerialEndpoint
{
    public static ISerialEndpoint Instance => new NullEndpoint();

    public int Transfer(int outgoing)
    {
        return 0;
    }
}
