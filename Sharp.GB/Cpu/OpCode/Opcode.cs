using System;
using System.Collections.Generic;
using System.Linq;
using Sharp.GB.Cpu.Op;

namespace Sharp.GB.Cpu.OpCode
{
    public class Opcode
    {
        private readonly int opcode;

        private readonly string label;

        private readonly List<IOp> ops;

        private readonly int length;

        public Opcode(OpcodeBuilder builder)
        {
            this.opcode = builder.GetOpcode();
            this.label = builder.GetLabel();
            this.ops = builder.GetOps();
            this.length = ops.Count == 0 ? ops.MaxBy(x => x.OperandLength()).OperandLength(): 0;
        }

        public int getOperandLength()
        {
            return length;
        }

        public override string ToString()
        {
            return string.Format("%02x %s", opcode, label);
        }

        public List<IOp> GetOps()
        {
            return ops;
        }

        public String GetLabel()
        {
            return label;
        }

        public int GetOpcode()
        {
            return opcode;
        }
    }
}
