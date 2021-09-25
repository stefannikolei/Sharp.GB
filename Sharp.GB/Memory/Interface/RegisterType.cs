namespace Sharp.GB.Memory.Interface
{
    public class RegisterType
    {
        public bool AllowsRead { get; }
        public bool AllowsWrite { get; }

        public static RegisterType R() => new RegisterType(true, false);
        public static RegisterType W() => new RegisterType(false, true);
        public static RegisterType RW() => new RegisterType(true, true);

        private RegisterType(bool allowsRead, bool allowsWrite)
        {
            AllowsRead = allowsRead;
            AllowsWrite = allowsWrite;
        }
    }
}
