using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Gpu
{
    public class GpuRegister : IRegister
    {
        public static GpuRegister Stat => new GpuRegister(0xff41, RegisterType.Rw());
        public static GpuRegister Scy => new GpuRegister(0xff42, RegisterType.Rw());
        public static GpuRegister Scx => new GpuRegister(0xff43, RegisterType.Rw());
        public static GpuRegister Ly => new GpuRegister(0xff44, RegisterType.R());
        public static GpuRegister Lyc => new GpuRegister(0xff45, RegisterType.Rw());
        public static GpuRegister Bgp => new GpuRegister(0xff47, RegisterType.Rw());
        public static GpuRegister Obp0 => new GpuRegister(0xff48, RegisterType.Rw());
        public static GpuRegister Obp1 => new GpuRegister(0xff49, RegisterType.Rw());
        public static GpuRegister Wy => new GpuRegister(0xff4a, RegisterType.Rw());
        public static GpuRegister Wx => new GpuRegister(0xffab, RegisterType.Rw());
        public static GpuRegister Vbk => new GpuRegister(0xff4f, RegisterType.W());

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
            return
            [
                Stat,
                Scy,
                Scx,
                Ly,
                Lyc,
                Bgp,
                Obp0,
                Obp1,
                Wy,
                Wx,
                Vbk
            ];
        }
    }
}
