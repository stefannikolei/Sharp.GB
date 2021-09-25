using System.Text;
using Sharp.GB.Common;

namespace Sharp.GB.Cpu
{
    public class Flags
    {
        private static int Z_POS = 7;

        private static int N_POS = 6;

        private static int H_POS = 5;

        private static int C_POS = 4;

        private int flags;

        public int getFlagsByte() {
            return flags;
        }

        public bool isZ() {
            return BitUtils.getBit(flags, Z_POS);
        }

        public bool isN() {
            return BitUtils.getBit(flags, N_POS);
        }

        public bool isH() {
            return BitUtils.getBit(flags, H_POS);
        }

        public bool isC() {
            return BitUtils.getBit(flags, C_POS);
        }

        public void setZ(bool z) {
            flags = BitUtils.setBit(flags, Z_POS, z);
        }

        public void setN(bool n) {
            flags = BitUtils.setBit(flags, N_POS, n);
        }

        public void setH(bool h) {
            flags = BitUtils.setBit(flags, H_POS, h);
        }

        public void setC(bool c) {
            flags = BitUtils.setBit(flags, C_POS, c);
        }

        public void setFlagsByte(int flags) {
            //checkByteArgument("flags", flags);
            this.flags = flags & 0xf0;
        }

        public string ToString() {
            StringBuilder result = new StringBuilder();
            result.Append(isZ() ? 'Z' : '-');
            result.Append(isN() ? 'N' : '-');
            result.Append(isH() ? 'H' : '-');
            result.Append(isC() ? 'C' : '-');
            result.Append("----");
            return result.ToString();
        }
    }
}
