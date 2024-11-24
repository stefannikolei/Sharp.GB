namespace Sharp.GB.Memory.Interface
{
    public class RegisterType
    {
        public bool AllowsRead { get; }
        public bool AllowsWrite { get; }

        public static RegisterType R() => new(true, false);
        public static RegisterType W() => new(false, true);
        public static RegisterType Rw() => new(true, true);

        private RegisterType(bool allowsRead, bool allowsWrite)
        {
            AllowsRead = allowsRead;
            AllowsWrite = allowsWrite;
        }
    }
}
