using Sharp.GB.Cpu;
using Sharp.GB.Cpu.OpCode;

namespace Sharp.GB.Debug.Commands.Cpu
{
    public class ShowOpcodes : ICommand
    {
        private static readonly CommandPattern s_pattern = CommandPattern
            .Builder.Create("cpu show opcodes")
            .WithDescription("displays all opcodes")
            .Build();

        public CommandPattern GetPattern()
        {
            return s_pattern;
        }

        public void Run(CommandPattern.ParsedCommandLine commandLine)
        {
            PrintTable(OpCodes.Commands);
            System.Console.WriteLine("\n0xCB");
            PrintTable(OpCodes.ExtCommands);
        }

        private static void PrintTable(List<Opcode?> opcodes)
        {
            System.Console.Write("   ");
            for (int i = 0; i < 0x10; i++)
            {
                System.Console.Write($"{i:X2}          ");
            }
            System.Console.WriteLine();

            for (int i = 0; i < 0x100; i += 0x10)
            {
                System.Console.Write($"{i:X2} ");
                for (int j = 0; j < 0x10; j++)
                {
                    Opcode? opcode = opcodes[i + j];
                    string label = opcode == null ? "-" : opcode.GetLabel();
                    System.Console.Write($"{label, -12}");
                }
                System.Console.WriteLine();
            }
        }
    }
}
