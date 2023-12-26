using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Sharp.GB.Common;
using Sharp.GB.Cpu.Op;
using Sharp.GB.Cpu.Op.ArgumentImplementations;
using Sharp.GB.Cpu.Op.Ops;
using Sharp.GB.Gpu;

namespace Sharp.GB.Cpu.OpCode
{
    public class OpcodeBuilder
    {
        private static readonly AluFunctions ALU = new AluFunctions();

        private static readonly ImmutableHashSet<Func<Flags, int, int>> OEM_BUG;

        static OpcodeBuilder()
        {
            HashSet<Func<Flags, int, int>> oemBugFunctions = new HashSet<Func<Flags, int, int>>();
            oemBugFunctions.Add(ALU.findAluFunction("INC", DataType.D16));
            oemBugFunctions.Add(ALU.findAluFunction("DEC", DataType.D16));
            OEM_BUG = oemBugFunctions.ToImmutableHashSet();
        }

        private readonly int opcode;

        private readonly string label;

        private readonly List<IOp> ops = new();

        private DataType lastDataType;

        public OpcodeBuilder(int opcode, string label)
        {
            this.opcode = opcode;
            this.label = label;
        }

        public OpcodeBuilder copyByte(string target, string source)
        {
            load(source);
            store(target);
            return this;
        }

        public OpcodeBuilder load(string source)
        {
            Argument arg = Argument.parse(source);
            lastDataType = arg.getDataType();
            ops.Add(new BasicOp(arg));
            return this;
        }

        public OpcodeBuilder loadWord(int value)
        {
            lastDataType = DataType.D16;
            ops.Add(new WordOp(value));
            return this;
        }

        public OpcodeBuilder store(string target)
        {
            Argument arg = Argument.parse(target);
            if (lastDataType == DataType.D16 && arg is _A16)
            {
                ops.Add(new _A16Op1(arg));
                ops.Add(new _A16Op2(arg));
            }
            else if (lastDataType == arg.getDataType())
            {
                ops.Add(new WriteOp(arg));
            }
            else
            {
                throw new ArgumentOutOfRangeException("Can't write " + lastDataType + " to " + target);
            }

            return this;
        }

        public OpcodeBuilder proceedIf(string condition)
        {
            ops.Add(new ProceedOp(condition));
            return this;
        }

        public OpcodeBuilder push()
        {
            var dec = ALU.findAluFunction("DEC", DataType.D16);
            ops.Add(new PushOp(dec));
            ops.Add(new PushOp2(dec));
            return this;
        }

        public OpcodeBuilder pop()
        {
            var inc = ALU.findAluFunction("INC", DataType.D16);

            lastDataType = DataType.D16;
            ops.Add(new PopOp1(inc));
            ops.Add(new PopOp2(inc));
            return this;
        }

        public OpcodeBuilder alu(String operation, String argument2)
        {
            Argument arg2 = Argument.parse(argument2);
            var func = ALU.findAluFunction(operation, lastDataType, arg2.getDataType());
            ops.Add(new AluOp(func, arg2, operation, lastDataType));

            if (lastDataType == DataType.D16)
            {
                extraCycle();
            }

            return this;
        }

        public OpcodeBuilder alu(String operation, int d8Value)
        {
            var func = ALU.findAluFunction(operation, lastDataType, DataType.D8);
            ops.Add(new AluD8Op(func, operation, d8Value));
            if (lastDataType == DataType.D16)
            {
                extraCycle();
            }

            return this;
        }

        public OpcodeBuilder alu(String operation)
        {
            var func = ALU.findAluFunction(operation, lastDataType);
            ops.Add(new AluOperationOp(func, operation, lastDataType));
            if (lastDataType == DataType.D16)
            {
                extraCycle();
            }

            return this;
        }

        public OpcodeBuilder aluHL(String operation)
        {
            load("HL");
            AluFunctions.IntRegistryFunction func = ALU.findAluFunction(operation, DataType.D16);
            ops.add(new Op()
            {
                @Override
                public int execute(Registers registers,
                AddressSpace addressSpace,
                int[] args,
                int value) {
                return func.apply(registers.getFlags(),
                value);
            }

            @Override

            public SpriteBug.CorruptionType causesOemBug(Registers registers, int context)
            {
                return OpcodeBuilder.causesOemBug(func, context) ? SpriteBug.CorruptionType.LD_HL : null;
            }

            @Override

            public String toString()
            {
                return String.format("%s(HL) → [__]");
            }

            });
            store("HL");
            return this;
        }

        public OpcodeBuilder bitHL(int bit)
        {
            ops.add(new Op()
            {
                @Override
                public boolean readsMemory() {
                return true;
            }

            @Override

            public int execute(Registers registers, AddressSpace addressSpace, int[] args, int context)
            {
                int value = addressSpace.getByte(registers.getHL());
                Flags flags = registers.getFlags();
                flags.setN(false);
                flags.setH(true);
                if (bit < 8)
                {
                    flags.setZ(!BitUtils.getBit(value, bit));
                }

                return context;
            }

            @Override

            public String toString()
            {
                return String.format("BIT(%d,HL)", bit);
            }

            });
            return this;
        }

        public OpcodeBuilder clearZ()
        {
            ops.add(new Op()
            {
                @Override
                public int execute(Registers registers,
                AddressSpace addressSpace,
                int[] args,
                int context) {
                registers.getFlags().setZ(false);
                return context;
            }

            @Override

            public String toString()
            {
                return String.format("0 → Z");
            }

            });
            return this;
        }

        public OpcodeBuilder switchInterrupts(bool enable, bool withDelay)
        {
            ops.add(new Op()
            {
                @Override
                public void switchInterrupts(InterruptManager interruptManager) {
                if (enable) {
                interruptManager.enableInterrupts(withDelay);
            } else {
                interruptManager.disableInterrupts(withDelay);
            }
            }

            @Override

            public String toString()
            {
                return (enable ? "enable" : "disable") + " interrupts";
            }

            });
            return this;
        }

        public OpcodeBuilder op(Op op)
        {
            ops.add(op);
            return this;
        }

        public OpcodeBuilder extraCycle()
        {
            ops.add(new Op()
            {
                @Override
                public boolean readsMemory() {
                return true;
            }

            @Override

            public String toString()
            {
                return "wait cycle";
            }

            });
            return this;
        }

        public OpcodeBuilder forceFinish()
        {
            ops.add(new Op()
            {
                @Override
                public boolean forceFinishCycle() {
                return true;
            }

            @Override

            public String toString()
            {
                return "finish cycle";
            }

            });
            return this;
        }

        public Opcode build()
        {
            return new Opcode(this);
        }

        public int GetOpcode()
        {
            return opcode;
        }

        public string GetLabel()
        {
            return label;
        }

        public List<IOp> GetOps()
        {
            return ops;
        }


        public String toString()
        {
            return label;
        }

        public static bool causesOemBug(Func<Flags, int, int> function, int context)
        {
            return OEM_BUG.Contains(function) && inOamArea(context);
        }

        private static bool inOamArea(int address)
        {
            return address >= 0xfe00 && address <= 0xfeff;
        }
    }
}
