using System;
using System.Collections.Generic;
using System.Linq;
using Sharp.GB.Cpu.OpCode;

namespace Sharp.GB.Cpu
{
    public class OpCodes
    {
        public static List<Opcode?> Commands;

        public static List<Opcode?> ExtCommands;

        static OpCodes()
        {
            OpcodeBuilder[] opcodes = new OpcodeBuilder[0x100];
            OpcodeBuilder[] extOpcodes = new OpcodeBuilder[0x100];

            RegCmd(opcodes, 0x00, "NOP");

            foreach (var t in IndexedList<string>(0x01, 0x10, "BC", "DE", "HL", "SP"))
            {
                RegLoad(opcodes, t.Key, t.Value, "d16");
            }

            foreach (var t in IndexedList(0x02, 0x10, "(BC)", "(DE)"))
            {
                RegLoad(opcodes, t.Key, t.Value, "A");
            }

            foreach (var t in IndexedList(0x03, 0x10, "BC", "DE", "HL", "SP"))
            {
                RegCmd(opcodes, t, "INC {}").Load(t.Value).Alu("INC").Store(t.Value);
            }

            foreach (var t in IndexedList(0x04, 0x08, "B", "C", "D", "E", "H", "L", "(HL)", "A"))
            {
                RegCmd(opcodes, t, "INC {}").Load(t.Value).Alu("INC").Store(t.Value);
            }

            foreach (var t in IndexedList(0x05, 0x08, "B", "C", "D", "E", "H", "L", "(HL)", "A"))
            {
                RegCmd(opcodes, t, "DEC {}").Load(t.Value).Alu("DEC").Store(t.Value);
            }

            foreach (var t in IndexedList(0x06, 0x08, "B", "C", "D", "E", "H", "L", "(HL)", "A"))
            {
                RegLoad(opcodes, t.Key, t.Value, "d8");
            }

            foreach (var o in IndexedList(0x07, 0x08, "RLC", "RRC", "RL", "RR"))
            {
                RegCmd(opcodes, o, o.Value + "A").Load("A").Alu(o.Value).ClearZ().Store("A");
            }

            RegLoad(opcodes, 0x08, "(a16)", "SP");

            foreach (var t in IndexedList(0x09, 0x10, "BC", "DE", "HL", "SP"))
            {
                RegCmd(opcodes, t, "ADD HL,{}").Load("HL").Alu("ADD", t.Value).Store("HL");
            }

            foreach (var t in IndexedList(0x0a, 0x10, "(BC)", "(DE)"))
            {
                RegLoad(opcodes, t.Key, "A", t.Value);
            }

            foreach (var t in IndexedList(0x0b, 0x10, "BC", "DE", "HL", "SP"))
            {
                RegCmd(opcodes, t, "DEC {}").Load(t.Value).Alu("DEC").Store(t.Value);
            }

            RegCmd(opcodes, 0x10, "STOP");

            RegCmd(opcodes, 0x18, "JR r8").Load("PC").Alu("ADD", "r8").Store("PC");

            foreach (var c in IndexedList(0x20, 0x08, "NZ", "Z", "NC", "C"))
            {
                RegCmd(opcodes, c, "JR {},r8")
                    .Load("PC")
                    .ProceedIf(c.Value)
                    .Alu("ADD", "r8")
                    .Store("PC");
            }

            RegCmd(opcodes, 0x22, "LD (HL+),A").CopyByte("(HL)", "A").AluHl("INC");
            RegCmd(opcodes, 0x2a, "LD A,(HL+)").CopyByte("A", "(HL)").AluHl("INC");

            RegCmd(opcodes, 0x27, "DAA").Load("A").Alu("DAA").Store("A");
            RegCmd(opcodes, 0x2f, "CPL").Load("A").Alu("CPL").Store("A");

            RegCmd(opcodes, 0x32, "LD (HL-),A").CopyByte("(HL)", "A").AluHl("DEC");
            RegCmd(opcodes, 0x3a, "LD A,(HL-)").CopyByte("A", "(HL)").AluHl("DEC");

            RegCmd(opcodes, 0x37, "SCF").Load("A").Alu("SCF").Store("A");
            RegCmd(opcodes, 0x3f, "CCF").Load("A").Alu("CCF").Store("A");

            foreach (var t in IndexedList(0x40, 0x08, "B", "C", "D", "E", "H", "L", "(HL)", "A"))
            {
                foreach (
                    var s in IndexedList(t.Key, 0x01, "B", "C", "D", "E", "H", "L", "(HL)", "A")
                )
                {
                    if (s.Key == 0x76)
                    {
                        continue;
                    }

                    RegLoad(opcodes, s.Key, t.Value, s.Value);
                }
            }

            RegCmd(opcodes, 0x76, "HALT");

            foreach (
                var o in IndexedList(
                    0x80,
                    0x08,
                    "ADD",
                    "ADC",
                    "SUB",
                    "SBC",
                    "AND",
                    "XOR",
                    "OR",
                    "CP"
                )
            )
            {
                foreach (
                    var t in IndexedList(o.Key, 0x01, "B", "C", "D", "E", "H", "L", "(HL)", "A")
                )
                {
                    RegCmd(opcodes, t, o.Value + " {}").Load("A").Alu(o.Value, t.Value).Store("A");
                }
            }

            foreach (var c in IndexedList(0xc0, 0x08, "NZ", "Z", "NC", "C"))
            {
                RegCmd(opcodes, c, "RET {}")
                    .ExtraCycle()
                    .ProceedIf(c.Value)
                    .Pop()
                    .ForceFinish()
                    .Store("PC");
            }

            foreach (var t in IndexedList(0xc1, 0x10, "BC", "DE", "HL", "AF"))
            {
                RegCmd(opcodes, t, "POP {}").Pop().Store(t.Value);
            }

            foreach (var c in IndexedList(0xc2, 0x08, "NZ", "Z", "NC", "C"))
            {
                RegCmd(opcodes, c, "JP {},a16")
                    .Load("a16")
                    .ProceedIf(c.Value)
                    .Store("PC")
                    .ExtraCycle();
            }

            RegCmd(opcodes, 0xc3, "JP a16").Load("a16").Store("PC").ExtraCycle();

            foreach (var c in IndexedList(0xc4, 0x08, "NZ", "Z", "NC", "C"))
            {
                RegCmd(opcodes, c, "CALL {},a16")
                    .ProceedIf(c.Value)
                    .ExtraCycle()
                    .Load("PC")
                    .Push()
                    .Load("a16")
                    .Store("PC");
            }

            foreach (var t in IndexedList(0xc5, 0x10, "BC", "DE", "HL", "AF"))
            {
                RegCmd(opcodes, t, "PUSH {}").ExtraCycle().Load(t.Value).Push();
            }

            foreach (
                var o in IndexedList(
                    0xc6,
                    0x08,
                    "ADD",
                    "ADC",
                    "SUB",
                    "SBC",
                    "AND",
                    "XOR",
                    "OR",
                    "CP"
                )
            )
            {
                RegCmd(opcodes, o, o.Value + " d8").Load("A").Alu(o.Value, "d8").Store("A");
            }

            for (int i = 0xc7, j = 0x00; i <= 0xf7; i += 0x10, j += 0x10)
            {
                // TODO: Probably string.Format is wrong here
                RegCmd(opcodes, i, string.Format("RST %02XH", j))
                    .Load("PC")
                    .Push()
                    .ForceFinish()
                    .LoadWord(j)
                    .Store("PC");
            }

            RegCmd(opcodes, 0xc9, "RET").Pop().ForceFinish().Store("PC");

            RegCmd(opcodes, 0xcd, "CALL a16")
                .Load("PC")
                .ExtraCycle()
                .Push()
                .Load("a16")
                .Store("PC");

            for (int i = 0xcf, j = 0x08; i <= 0xff; i += 0x10, j += 0x10)
            {
                RegCmd(opcodes, i, string.Format("RST %02XH", j))
                    .Load("PC")
                    .Push()
                    .ForceFinish()
                    .LoadWord(j)
                    .Store("PC");
            }

            RegCmd(opcodes, 0xd9, "RETI")
                .Pop()
                .ForceFinish()
                .Store("PC")
                .SwitchInterrupts(true, false);

            RegLoad(opcodes, 0xe2, "(C)", "A");
            RegLoad(opcodes, 0xf2, "A", "(C)");

            RegCmd(opcodes, 0xe9, "JP (HL)").Load("HL").Store("PC");

            RegCmd(opcodes, 0xe0, "LDH (a8),A").CopyByte("(a8)", "A");
            RegCmd(opcodes, 0xf0, "LDH A,(a8)").CopyByte("A", "(a8)");

            RegCmd(opcodes, 0xe8, "ADD SP,r8")
                .Load("SP")
                .Alu("ADD_SP", "r8")
                .ExtraCycle()
                .Store("SP");
            RegCmd(opcodes, 0xf8, "LD HL,SP+r8").Load("SP").Alu("ADD_SP", "r8").Store("HL");

            RegLoad(opcodes, 0xea, "(a16)", "A");
            RegLoad(opcodes, 0xfa, "A", "(a16)");

            RegCmd(opcodes, 0xf3, "DI").SwitchInterrupts(false, true);
            RegCmd(opcodes, 0xfb, "EI").SwitchInterrupts(true, true);

            RegLoad(opcodes, 0xf9, "SP", "HL").ExtraCycle();

            foreach (
                var o in IndexedList(
                    0x00,
                    0x08,
                    "RLC",
                    "RRC",
                    "RL",
                    "RR",
                    "SLA",
                    "SRA",
                    "SWAP",
                    "SRL"
                )
            )
            {
                foreach (
                    var t in IndexedList(o.Key, 0x01, "B", "C", "D", "E", "H", "L", "(HL)", "A")
                )
                {
                    RegCmd(extOpcodes, t, o.Value + " {}")
                        .Load(t.Value)
                        .Alu(o.Value)
                        .Store(t.Value);
                }
            }

            foreach (var o in IndexedList(0x40, 0x40, "BIT", "RES", "SET"))
            {
                for (int b = 0; b < 0x08; b++)
                {
                    foreach (
                        var t in IndexedList(
                            o.Key + b * 0x08,
                            0x01,
                            "B",
                            "C",
                            "D",
                            "E",
                            "H",
                            "L",
                            "(HL)",
                            "A"
                        )
                    )
                    {
                        if ("BIT".Equals(o.Value) && "(HL)".Equals(t.Value))
                        {
                            RegCmd(extOpcodes, t, string.Format("BIT %d,(HL)", b)).BitHl(b);
                        }
                        else
                        {
                            RegCmd(extOpcodes, t, string.Format("%s %d,%s", o.Value, b, t.Value))
                                .Load(t.Value)
                                .Alu(o.Value, b)
                                .Store(t.Value);
                        }
                    }
                }
            }

            List<Opcode?> commands = new(0x100);
            List<Opcode?> extCommands = new(0x100);

            foreach (OpcodeBuilder b in opcodes)
            {
                if (b == null)
                {
                    commands.Add(null);
                }
                else
                {
                    commands.Add(b.Build());
                }
            }

            foreach (OpcodeBuilder b in extOpcodes)
            {
                if (b == null)
                {
                    extCommands.Add(null);
                }
                else
                {
                    extCommands.Add(b.Build());
                }
            }

            Commands = commands.ToList();
            ExtCommands = extCommands.ToList();
        }

        private static OpcodeBuilder RegLoad(
            OpcodeBuilder[] commands,
            int opcode,
            string target,
            string source
        )
        {
            return RegCmd(commands, opcode, string.Format("LD %s,%s", target, source))
                .CopyByte(target, source);
        }

        private static OpcodeBuilder RegCmd(OpcodeBuilder[] commands, int opcode, string label)
        {
            if (commands[opcode] != null)
            {
                throw new ArgumentException(
                    string.Format("Opcode %02X already exists: %s", opcode, commands[opcode])
                );
            }

            OpcodeBuilder builder = new OpcodeBuilder(opcode, label);
            commands[opcode] = builder;
            return builder;
        }

        private static OpcodeBuilder RegCmd(
            OpcodeBuilder[] commands,
            KeyValuePair<int, string> opcode,
            string label
        )
        {
            return RegCmd(commands, opcode.Key, label.Replace("{}", opcode.Value));
        }

        private static Dictionary<int, T> IndexedList<T>(int start, int step, params T[] values)
        {
            Dictionary<int, T> map = new();
            int i = start;
            foreach (T e in values)
            {
                map.Add(i, e);
                i += step;
            }

            return map;
        }
    }

    public interface IOpcodeBuilder { }
}
