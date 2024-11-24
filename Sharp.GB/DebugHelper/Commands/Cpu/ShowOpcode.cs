using System;
using System.Collections.Generic;
using System.Linq;
using Sharp.GB.Cpu;
using Sharp.GB.Cpu.Op;
using Sharp.GB.Cpu.OpCode;

namespace Sharp.GB.Debug.Commands.Cpu;

public class ShowOpcode : ICommand
{
    private static readonly CommandPattern s_pattern = CommandPattern
        .Builder.Create("cpu show opcode")
        .WithRequiredArgument("opcode")
        .WithDescription("displays opcode information for hex (0xFA) or name (LD A,B) identifier")
        .Build();

    public CommandPattern GetPattern()
    {
        return s_pattern;
    }

    public void Run(CommandPattern.ParsedCommandLine commandLine)
    {
        string arg;
        if (!commandLine.GetRemainingArguments().Any())
        {
            arg = commandLine.GetArgument("opcode");
        }
        else
        {
            arg =
                commandLine.GetArgument("opcode")
                + " "
                + string.Join(" ", commandLine.GetRemainingArguments());
        }

        Opcode? opcode = GetOpcodeFromArg(arg);
        if (opcode == null)
        {
            Console.WriteLine("Can't found opcode for " + arg);
            return;
        }

        bool isExt = OpCodes.ExtCommands[opcode.GetOpcode()] == opcode;

        List<OpDescription> ops = new();
        Action<int, string> addOp = (c, d) => ops.Add(new(c, d));
        if (isExt)
        {
            addOp.Invoke(4, "read opcode 0xCB");
        }
        addOp.Invoke(4, $"read opcode 0x{opcode.GetOpcode():X2}");
        for (int i = 0; i < opcode.GetOperandLength(); i++)
        {
            addOp.Invoke(4, $"read operand {i + 1}");
        }
        ops.AddRange(opcode.GetOps().Select(op => new OpDescription(op)));

        List<OpDescription> compacted = new();
        for (int i = 0; i < ops.Count; i++)
        {
            OpDescription o = ops[i];
            if (o.Description == "wait cycle")
            {
                if (compacted.Any())
                {
                    compacted.Last().UpdateCycles(4);
                }
            }
            else if (o.Description == "finish cycle")
            {
                if (i < ops.Count - 1)
                {
                    OpDescription nextOp = ops[++i];
                    nextOp.UpdateCycles(4);
                    compacted.Add(nextOp);
                }
            }
            else
            {
                compacted.Add(o);
            }
        }

        int stringLength = compacted.Select(c => c.ToString()).Max(s => s.Length);

        int totalCycles = compacted.Sum(o => o.Cycles);
        bool conditionalOccurred = false;
        int totalCyclesUntilCondition = compacted
            .Where(o =>
            {
                var prev = conditionalOccurred;
                conditionalOccurred = conditionalOccurred || o.Description.StartsWith("? ");
                return !prev;
            })
            .Sum(o => o.Cycles);

        if (isExt)
        {
            Console.WriteLine($"0xCB{opcode.GetOpcode():X2} {opcode.GetLabel()}");
        }
        else
        {
            Console.WriteLine($"0x{opcode.GetOpcode():X2} {opcode.GetLabel()}");
        }
        ConsoleUtil.PrintSeparator(stringLength);
        compacted.ForEach(Console.WriteLine);
        ConsoleUtil.PrintSeparator(stringLength);
        if (totalCyclesUntilCondition != totalCycles)
        {
            Console.WriteLine($"Total cycles: {totalCycles} / {totalCyclesUntilCondition}");
        }
        else
        {
            Console.WriteLine($"Total cycles: {totalCycles}");
        }
    }

    private Opcode? GetOpcodeFromArg(string arg)
    {
        if (System.Text.RegularExpressions.Regex.IsMatch(arg.ToLower(), "0x[0-9a-f]{2}"))
        {
            return GetFromHex(OpCodes.Commands, arg.Substring(2));
        }
        else if (System.Text.RegularExpressions.Regex.IsMatch(arg.ToLower(), "0xcb[0-9a-f]{2}"))
        {
            return GetFromHex(OpCodes.ExtCommands, arg.Substring(4));
        }
        else if (System.Text.RegularExpressions.Regex.IsMatch(arg.ToLower(), "[0-9a-f]{2}"))
        {
            return GetFromHex(OpCodes.Commands, arg);
        }
        else if (System.Text.RegularExpressions.Regex.IsMatch(arg.ToLower(), "cb[0-9a-f]{2}"))
        {
            return GetFromHex(OpCodes.ExtCommands, arg.Substring(2));
        }

        string compactedArg = CompactOpcodeLabel(arg);
        Opcode? opcode = OpCodes.Commands.FirstOrDefault(o =>
            o != null && compactedArg == CompactOpcodeLabel(o.GetLabel())
        );
        if (opcode == null)
        {
            opcode = OpCodes.ExtCommands.FirstOrDefault(o =>
                o != null && compactedArg == CompactOpcodeLabel(o.GetLabel())
            );
        }
        return opcode!;
    }

    private Opcode? GetFromHex(List<Opcode?> opcodes, string hexArg)
    {
        return opcodes[Convert.ToInt32(hexArg, 16)];
    }

    private string CompactOpcodeLabel(string label)
    {
        return label.Replace(" ", "").ToLower();
    }

    public class OpDescription
    {
        public string Description { get; private set; }
        public int Cycles { get; private set; }

        public OpDescription(IOp op)
        {
            Description = op.ToString()!;
            if (op.WritesMemory() || op.ReadsMemory())
            {
                Cycles = 4;
            }
            else
            {
                Cycles = 0;
            }
        }

        public OpDescription(int cycles, string description)
        {
            Description = description;
            Cycles = cycles;
        }

        public void UpdateCycles(int cycles)
        {
            Cycles += cycles;
        }

        public override string ToString()
        {
            return $"{(Cycles == 0 ? " " : Cycles.ToString()), -4}   {Description}";
        }
    }
}
