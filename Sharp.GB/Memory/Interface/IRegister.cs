namespace Sharp.GB.Memory.Interface
{
    public interface IRegister
    {
        int GetAddress();

        RegisterType Type { get; }
    }
}
