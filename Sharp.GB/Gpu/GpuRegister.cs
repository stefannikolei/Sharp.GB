using Sharp.GB.Memory;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Gpu
{
    public class GpuRegister : IRegister
    {
        public static GpuRegister STAT() => new GpuRegister(0xff41, RegisterType.RW());
        public static GpuRegister SCY() => new GpuRegister(0xff42, RegisterType.RW());
        public static GpuRegister SCX() => new GpuRegister(0xff43, RegisterType.RW());
        public static GpuRegister LY() => new GpuRegister(0xff44, RegisterType.R());
        public static GpuRegister LYC() => new GpuRegister(0xff45, RegisterType.RW());
        public static GpuRegister BGP() => new GpuRegister(0xff47, RegisterType.RW());
        public static GpuRegister OBP0() => new GpuRegister(0xff48, RegisterType.RW());
        public static GpuRegister OBP1() => new GpuRegister(0xff49, RegisterType.RW());
        public static GpuRegister WY() => new GpuRegister(0xff4a, RegisterType.RW());
        public static GpuRegister WX() => new GpuRegister(0xffab, RegisterType.RW());
        public static GpuRegister VBK() => new GpuRegister(0xff4f, RegisterType.W());

        private int _address;
        private RegisterType _type;

        private GpuRegister(int address, RegisterType type)
        {
            _address = address;
            _type = type;
        }

        public int GetAddress()
        {
            return _address;
        }

        RegisterType IRegister.Type => _type;

        public static IRegister[] GetAll()
        {
            return new[] {STAT(), SCY(), SCX(), LY(), LYC(), BGP(), OBP0(), OBP1(), WY(), WX(), VBK()};
        }
    }
}
