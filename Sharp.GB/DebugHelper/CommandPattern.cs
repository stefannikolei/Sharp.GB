using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharp.GB.Debug;

public class CommandPattern
{
    private readonly List<string> _commandNames;

    private readonly List<CommandArgument> _arguments;

    private readonly string? _description;

    private CommandPattern(Builder builder)
    {
        _commandNames = builder.CommandNames;
        _arguments = builder.Arguments;
        _description = builder._description;
    }

    public bool Matches(string commandLine)
    {
        var first = _commandNames.First(x => x.StartsWith(commandLine));

        return string.IsNullOrEmpty(first) || first[0] == ' ';
    }

    public List<string> GetCommandNames()
    {
        return _commandNames;
    }

    public List<CommandArgument> GetArguments()
    {
        return _arguments;
    }

    public string? GetDescription()
    {
        return _description;
    }

    public ParsedCommandLine Parse(string commandLine)
    {
        string commandName = _commandNames.First(x => x.StartsWith(commandLine));

        List<string> split = Split(commandLine.Substring(commandName.Length));
        Dictionary<string, string> map = [];
        List<string> remaining = [];
        int i;
        for (i = 0; i < split.Count && i < _arguments.Count; i++)
        {
            string value = split[i];
            CommandArgument argDef = _arguments[i];
            List<string>? allowed = argDef.GetAllowedValues();
            if (allowed is not null)
            {
                if (!allowed.Contains(value))
                {
                    throw new ApplicationException(
                        "Value "
                            + value
                            + " is not allowed for argument "
                            + argDef.GetName()
                            + ". Allowed values: "
                            + allowed
                    );
                }
            }
            map.Add(argDef.GetName(), value);
        }
        if (i < _arguments.Count)
        {
            CommandArgument argDef = _arguments[i];
            if (argDef.IsRequired())
            {
                throw new ApplicationException("Argument " + argDef.GetName() + " is required");
            }
        }
        if (i < split.Count)
        {
            remaining = split.GetRange(i, split.Count);
        }
        return new(map, remaining);
    }

    private static List<string> Split(string str)
    {
        List<string> split = [];
        bool isEscaped = false;
        StringBuilder currentArg = new StringBuilder();
        for (int i = 0; i <= str.Length; i++)
        {
            char c;
            if (i < str.Length)
            {
                c = str[i];
            }
            else
            {
                c = '0';
            }

            switch (c)
            {
                case '"':
                    break;

                case ' ':
                case '0':
                    if (currentArg.Length > 0)
                    {
                        split.Add(currentArg.ToString());
                        currentArg.Clear();
                    }
                    break;

                default:
                    currentArg.Append(c);
                    break;
            }
        }
        return split;
    }

    public override string ToString()
    {
        return $"CommandPattern[{_commandNames.ToString()}]";
    }

    public class ParsedCommandLine
    {
        private Dictionary<string, string> _argumentMap;

        private List<string> _remainingArguments;

        public ParsedCommandLine(
            Dictionary<string, string> argumentMap,
            List<string> remainingArguments
        )
        {
            _argumentMap = argumentMap;
            _remainingArguments = remainingArguments;
        }

        public string GetArgument(string name)
        {
            return _argumentMap[name];
        }

        public List<string> GetRemainingArguments()
        {
            return _remainingArguments;
        }
    }

    public class Builder
    {
        public readonly List<string> CommandNames;

        public readonly List<CommandArgument> Arguments;

        public string? _description;

        private Builder(string[] commandNames)
        {
            CommandNames = commandNames.ToList();
            Arguments = [];
        }

        public static Builder Create(string longName)
        {
            return new([longName]);
        }

        public static Builder Create(string longName, string shortName)
        {
            return new([longName, shortName]);
        }

        public Builder WithOptionalArgument(string name)
        {
            AssertNoOptionalLastArgument();
            Arguments.Add(new(name, false));
            return this;
        }

        public Builder WithRequiredArgument(string name)
        {
            AssertNoOptionalLastArgument();
            Arguments.Add(new(name, true));
            return this;
        }

        public Builder WithOptionalValue(string name, params string[] values)
        {
            AssertNoOptionalLastArgument();
            Arguments.Add(new(name, false, values.ToList()));
            return this;
        }

        public Builder WithRequiredValue(string name, params string[] values)
        {
            AssertNoOptionalLastArgument();
            Arguments.Add(new(name, true, values.ToList()));
            return this;
        }

        public Builder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        private void AssertNoOptionalLastArgument()
        {
            if (!Arguments.Any() && !Arguments[Arguments.Count - 1].IsRequired())
            {
                throw new ApplicationException("Can't add argument after the optional one");
            }
        }

        public CommandPattern Build()
        {
            return new(this);
        }
    }
}
