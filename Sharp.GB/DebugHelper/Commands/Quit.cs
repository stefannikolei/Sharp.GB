using System;
using Sharp.GB.Debug;

public class Quit : ICommand {
    
    private static readonly CommandPattern s_pattern = CommandPattern.Builder
    .Create("quit", "q")
    .WithDescription("quits the emulator")
    .Build();

    public  CommandPattern GetPattern() {
        return s_pattern;
    }

    public  void Run(CommandPattern.ParsedCommandLine commandLine) {
        Environment.Exit(0);
    }
}
