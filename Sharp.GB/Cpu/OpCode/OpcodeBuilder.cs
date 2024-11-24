using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Sharp.GB.Cpu.Op;
using Sharp.GB.Cpu.Op.ArgumentImplementations;
using Sharp.GB.Cpu.Op.Ops;

namespace Sharp.GB.Cpu.OpCode
{
    public class OpcodeBuilder
    {
        private static readonly AluFunctions s_alu = new();

        private static readonly ImmutableHashSet<Func<Flags, int, int>> s_oemBug;

        static OpcodeBuilder()
        {
            HashSet<Func<Flags, int, int>> oemBugFunctions =
            [
                s_alu.FindAluFunction("INC", DataType.D16),
                s_alu.FindAluFunction("DEC", DataType.D16),
            ];
            s_oemBug = oemBugFunctions.ToImmutableHashSet();
        }

        private readonly int _opcode;

        private readonly string _label;

        private readonly List<IOp> _ops = new();

        private DataType _lastDataType;

        public OpcodeBuilder(int opcode, string label)
        {
            _opcode = opcode;
            _label = label;
        }

        public OpcodeBuilder CopyByte(string target, string source)
        {
            Load(source);
            Store(target);
            return this;
        }

        public OpcodeBuilder Load(string source)
        {
            Argument arg = Argument.Parse(source);
            _lastDataType = arg.GetDataType();
            _ops.Add(new BasicOp(arg));
            return this;
        }

        public OpcodeBuilder LoadWord(int value)
        {
            _lastDataType = DataType.D16;
            _ops.Add(new WordOp(value));
            return this;
        }

        public OpcodeBuilder Store(string target)
        {
            Argument arg = Argument.Parse(target);
            if (_lastDataType == DataType.D16 && arg is A162)
            {
                _ops.Add(new A16Op1(arg));
                _ops.Add(new A16Op2(arg));
            }
            else if (_lastDataType == arg.GetDataType())
            {
                _ops.Add(new WriteOp(arg));
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    "Can't write " + _lastDataType + " to " + target
                );
            }

            return this;
        }

        public OpcodeBuilder ProceedIf(string condition)
        {
            _ops.Add(new ProceedOp(condition));
            return this;
        }

        public OpcodeBuilder Push()
        {
            var dec = s_alu.FindAluFunction("DEC", DataType.D16);
            _ops.Add(new PushOp(dec));
            _ops.Add(new PushOp2(dec));
            return this;
        }

        public OpcodeBuilder Pop()
        {
            var inc = s_alu.FindAluFunction("INC", DataType.D16);

            _lastDataType = DataType.D16;
            _ops.Add(new PopOp1(inc));
            _ops.Add(new PopOp2(inc));
            return this;
        }

        public OpcodeBuilder Alu(string operation, string argument2)
        {
            Argument arg2 = Argument.Parse(argument2);
            var func = s_alu.FindAluFunction(operation, _lastDataType, arg2.GetDataType());
            _ops.Add(new AluOp(func, arg2, operation, _lastDataType));

            if (_lastDataType == DataType.D16)
            {
                ExtraCycle();
            }

            return this;
        }

        public OpcodeBuilder Alu(string operation, int d8Value)
        {
            var func = s_alu.FindAluFunction(operation, _lastDataType, DataType.D8);
            _ops.Add(new AluD8Op(func, operation, d8Value));
            if (_lastDataType == DataType.D16)
            {
                ExtraCycle();
            }

            return this;
        }

        public OpcodeBuilder Alu(string operation)
        {
            var func = s_alu.FindAluFunction(operation, _lastDataType);
            _ops.Add(new AluOperationOp(func, operation, _lastDataType));
            if (_lastDataType == DataType.D16)
            {
                ExtraCycle();
            }

            return this;
        }

        public OpcodeBuilder AluHl(string operation)
        {
            Load("HL");
            var func = s_alu.FindAluFunction(operation, DataType.D16);
            _ops.Add(new AluHlOp(func));
            Store("HL");
            return this;
        }

        public OpcodeBuilder BitHl(int bit)
        {
            _ops.Add(new BitHlOp(bit));
            return this;
        }

        public OpcodeBuilder ClearZ()
        {
            _ops.Add(new ClearZOp());
            return this;
        }

        public OpcodeBuilder SwitchInterrupts(bool enable, bool withDelay)
        {
            _ops.Add(new SwitchInterruptsOp(enable, withDelay));
            return this;
        }

        public OpcodeBuilder ExtraCycle()
        {
            _ops.Add(new ExtraCycleOp());
            return this;
        }

        public OpcodeBuilder ForceFinish()
        {
            _ops.Add(new ForceFinishOp());
            return this;
        }

        public Opcode Build()
        {
            return new(this);
        }

        public int GetOpcode()
        {
            return _opcode;
        }

        public string GetLabel()
        {
            return _label;
        }

        public List<IOp> GetOps()
        {
            return _ops;
        }

        public override string ToString()
        {
            return _label;
        }

        public static bool CausesOemBug(Func<Flags, int, int> function, int context)
        {
            return s_oemBug.Contains(function) && InOamArea(context);
        }

        private static bool InOamArea(int address)
        {
            return address >= 0xfe00 && address <= 0xfeff;
        }
    }
}
