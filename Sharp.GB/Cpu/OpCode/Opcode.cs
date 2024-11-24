using Sharp.GB.Cpu.Op;

namespace Sharp.GB.Cpu.OpCode
{
    public class Opcode
    {
        private readonly int _opcode;

        private readonly string _label;

        private readonly List<IOp> _ops;

        private readonly int _length;

        public Opcode(OpcodeBuilder builder)
        {
            _opcode = builder.GetOpcode();
            _label = builder.GetLabel();
            _ops = builder.GetOps();
            _length = _ops.Count != 0 ? _ops.MaxBy(x => x.OperandLength())!.OperandLength() : 0;
        }

        public int GetOperandLength()
        {
            return _length;
        }

        public override string ToString()
        {
            return $"{_opcode:x2} {_label}";
        }

        public List<IOp> GetOps()
        {
            return _ops;
        }

        public string GetLabel()
        {
            return _label;
        }

        public int GetOpcode()
        {
            return _opcode;
        }
    }
}
