using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharp.GB.Debug.Commands;

public class ShowHelp : ICommand
{
    private static readonly CommandPattern s_pattern = CommandPattern
        .Builder.Create("help", "?")
        .WithDescription("displays supported commands")
        .Build();

    private readonly List<ICommand> _commands;

    public ShowHelp(List<ICommand> commands)
    {
        this._commands = commands;
    }

    public CommandPattern GetPattern()
    {
        return s_pattern;
    }

    public void Run(CommandPattern.ParsedCommandLine commandLine)
    {
        int max = 0;
        Dictionary<ICommand, string> commandMap = new();
        foreach (ICommand command in _commands)
        {
            CommandPattern pattern = command.GetPattern();
            string alias = pattern.GetCommandNames()[0];
            string commandWithArgs = GetCommandWithArgs(alias, pattern.GetArguments());
            if (commandWithArgs.Length > max)
            {
                max = commandWithArgs.Length;
            }
            commandMap.Add(command, commandWithArgs);
        }

        foreach (ICommand command in _commands)
        {
            CommandPattern pattern = command.GetPattern();
            string longName = commandMap[command];
            Console.WriteLine(string.Format("%-" + max + "s", longName));
            if (pattern.GetCommandNames().Count > 1)
            {
                Console.WriteLine(string.Format("   %-5s", pattern.GetCommandNames()[1]));
            }
            else
            {
                Console.WriteLine("        ");
            }

            var a = command.GetPattern().GetDescription()?.Select(d => "   " + d);

            if (a != null)
            {
                Console.WriteLine(a);
            }
            Console.WriteLine();
        }
    }

    private string GetCommandWithArgs(string alias, List<CommandArgument> args)
    {
        StringBuilder builder = new StringBuilder(alias);
        if (!args.Any())
        {
            builder.Append(' ').Append(string.Join(" ", args));
        }
        return builder.ToString();
    }
}
