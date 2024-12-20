﻿using Sharp.GB.Common;

namespace Sharp.GB.Cpu
{
    public class Registers
    {
        private int _a,
            _b,
            _c,
            _d,
            _e,
            _h,
            _l;

        private int _sp;

        private int _pc;

        private Flags _flags = new();

        public int GetA()
        {
            return _a;
        }

        public int GetB()
        {
            return _b;
        }

        public int GetC()
        {
            return _c;
        }

        public int GetD()
        {
            return _d;
        }

        public int GetE()
        {
            return _e;
        }

        public int GetH()
        {
            return _h;
        }

        public int GetL()
        {
            return _l;
        }

        public int GetAf()
        {
            return _a << 8 | _flags.GetFlagsByte();
        }

        public int GetBc()
        {
            return _b << 8 | _c;
        }

        public int GetDe()
        {
            return _d << 8 | _e;
        }

        public int GetHl()
        {
            return _h << 8 | _l;
        }

        public int GetSp()
        {
            return _sp;
        }

        public int GetPc()
        {
            return _pc;
        }

        public Flags GetFlags()
        {
            return _flags;
        }

        public void SetA(int a)
        {
            // checkByteArgument("a", a);
            _a = a;
        }

        public void SetB(int b)
        {
            // checkByteArgument("b", b);
            _b = b;
        }

        public void SetC(int c)
        {
            // checkByteArgument("c", c);
            _c = c;
        }

        public void SetD(int d)
        {
            // checkByteArgument("d", d);
            _d = d;
        }

        public void SetE(int e)
        {
            // checkByteArgument("e", e);
            _e = e;
        }

        public void SetH(int h)
        {
            // checkByteArgument("h", h);
            _h = h;
        }

        public void SetL(int l)
        {
            // checkByteArgument("l", l);
            _l = l;
        }

        public void SetAf(int af)
        {
            // checkWordArgument("af", af);
            _a = BitUtils.GetMsb(af);
            _flags.SetFlagsByte(BitUtils.GetLsb(af));
        }

        public void SetBc(int bc)
        {
            // checkWordArgument("bc", bc);
            _b = BitUtils.GetMsb(bc);
            _c = BitUtils.GetLsb(bc);
        }

        public void SetDe(int de)
        {
            // checkWordArgument("de", de);
            _d = BitUtils.GetMsb(de);
            _e = BitUtils.GetLsb(de);
        }

        public void SetHl(int hl)
        {
            // checkWordArgument("hl", hl);
            _h = BitUtils.GetMsb(hl);
            _l = BitUtils.GetLsb(hl);
        }

        public void SetSp(int sp)
        {
            // checkWordArgument("sp", sp);
            _sp = sp;
        }

        public void SetPc(int pc)
        {
            // checkWordArgument("pc", pc);
            _pc = pc;
        }

        public void IncrementPc()
        {
            _pc = (_pc + 1) & 0xffff;
        }

        public void DecrementSp()
        {
            _sp = (_sp - 1) & 0xffff;
        }

        public override string ToString()
        {
            return string.Format(
                "AF={0:X4}, BC={1:X4}, DE={2:X4}, HL={3:X4}, SP={4:X4}, PC={5:X4}, {6}",
                GetAf(),
                GetBc(),
                GetDe(),
                GetHl(),
                GetSp(),
                GetPc(),
                GetFlags().ToString()
            );
        }
    }
}
