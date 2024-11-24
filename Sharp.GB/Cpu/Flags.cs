using System.Text;
using Sharp.GB.Common;

namespace Sharp.GB.Cpu
{
    public class Flags
    {
        private static int s_zPos = 7;

        private static int s_nPos = 6;

        private static int s_hPos = 5;

        private static int s_cPos = 4;

        private int _flags;

        public int GetFlagsByte()
        {
            return _flags;
        }

        public bool IsZ()
        {
            return BitUtils.GetBit(_flags, s_zPos);
        }

        public bool IsN()
        {
            return BitUtils.GetBit(_flags, s_nPos);
        }

        public bool IsH()
        {
            return BitUtils.GetBit(_flags, s_hPos);
        }

        public bool IsC()
        {
            return BitUtils.GetBit(_flags, s_cPos);
        }

        public void SetZ(bool z)
        {
            _flags = BitUtils.SetBit(_flags, s_zPos, z);
        }

        public void SetN(bool n)
        {
            _flags = BitUtils.SetBit(_flags, s_nPos, n);
        }

        public void SetH(bool h)
        {
            _flags = BitUtils.SetBit(_flags, s_hPos, h);
        }

        public void SetC(bool c)
        {
            _flags = BitUtils.SetBit(_flags, s_cPos, c);
        }

        public void SetFlagsByte(int flags)
        {
            //checkByteArgument("flags", flags);
            _flags = flags & 0xf0;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(IsZ() ? 'Z' : '-');
            result.Append(IsN() ? 'N' : '-');
            result.Append(IsH() ? 'H' : '-');
            result.Append(IsC() ? 'C' : '-');
            result.Append("----");
            return result.ToString();
        }
    }
}
