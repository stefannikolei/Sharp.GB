namespace Sharp.GB.Serial;

public interface ISerialEndpoint
{
    /**
        * Transfer the bit in the external clock mode (passive). Return the incoming bit or -1 if there's no bit to transfer.
        */
    int Receive(int bitToTransfer);

    /**
     * Transfer the bit in the internal clock mode (active). Return the incoming bit.
     */
    int Send(int bitToTransfer);
}

public class NullEndpoint : ISerialEndpoint
{
    public static ISerialEndpoint Instance => new NullEndpoint();

    public int Receive(int bitToTransfer)
    {
        return -1;
    }

    public int Send(int bitToTransfer)
    {
        return 0;
    }
}
