namespace Sharp.GB.Debug;

public interface ICommand
{
    CommandPattern GetPattern();

    void Run(CommandPattern.ParsedCommandLine commandLine);
}
