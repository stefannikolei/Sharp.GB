using System.Collections.Generic;
using Sharp.GB.Debug;
using Sharp.GB.Sound;

public class Channel : ICommand
{
    private static readonly CommandPattern s_pattern = CommandPattern
        .Builder.Create("apu chan")
        .WithDescription("enable given channels (1-4)")
        .Build();

    private Sound _sound;

    public Channel(Sound sound)
    {
        this._sound = sound;
    }

    public CommandPattern GetPattern()
    {
        return s_pattern;
    }

    public void Run(CommandPattern.ParsedCommandLine commandLine)
    {
        HashSet<string> channels = [..commandLine.GetRemainingArguments()];
        for (int i = 1; i <= 4; i++)
        {
            _sound.EnableChannel(i - 1, channels.Contains(i.ToString()));
        }
    }
}
