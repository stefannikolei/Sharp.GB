using Sharp.GB.Debug.Commands;
using Sharp.GB.Debug.Commands.Cpu;

namespace Sharp.GB.Debug;

public class GameboyConsole
{
    private readonly Queue<CommandExecution> _commandBuffer = [];

    private readonly SemaphoreSlim _semaphore = new(0);

    private volatile bool _isStarted;

    private List<ICommand> _commands;

    public GameboyConsole(Gameboy gameboy)
    {
        _commands = [];
        _commands.Add(new ShowHelp(_commands));
        _commands.Add(new ShowOpcode());
        _commands.Add(new ShowOpcodes());
        _commands.Add(new Quit());

        // commands.Add(new ShowBackground(gameboy, ShowBackground.Type.WINDOW));
        // commands.Add(new ShowBackground(gameboy, ShowBackground.Type.BACKGROUND));
        _commands.Add(new Channel(gameboy.GetSound()));
        // Collections.sort(commands, Comparator.comparing(c -> c.getPattern().getCommandNames().get(0)));
    }

    public void Run()
    {
        _isStarted = true;

        while (true)
        {
            try
            {
                Console.Write("coffee-gb> ");
                var line = Console.ReadLine();
                foreach (var cmd in _commands)
                {
                    if (line is null)
                    {
                        continue;
                    }
                    if (cmd.GetPattern().Matches(line))
                    {
                        CommandPattern.ParsedCommandLine parsed = cmd.GetPattern().Parse(line);
                        _commandBuffer.Enqueue(new(cmd, parsed));
                        _semaphore.Wait();
                    }
                }
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public void Tick()
    {
        if (!_isStarted)
        {
            return;
        }

        while (!_commandBuffer.Any())
        {
            _commandBuffer.Dequeue().Run();
            _semaphore.Release();
        }
    }

    private class CommandExecution
    {
        private readonly ICommand _command;

        private readonly CommandPattern.ParsedCommandLine _arguments;

        public CommandExecution(ICommand command, CommandPattern.ParsedCommandLine arguments)
        {
            _command = command;
            _arguments = arguments;
        }

        public void Run()
        {
            _command.Run(_arguments);
        }
    }
}
