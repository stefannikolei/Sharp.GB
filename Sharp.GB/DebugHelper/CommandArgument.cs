using System.Text;

namespace Sharp.GB.Debug;

public class CommandArgument
{
    private readonly string _name;

    private readonly bool _required;

    private readonly List<string>? _allowedValues;

    public CommandArgument(string name, bool required)
    {
        _name = name;
        _required = required;
        _allowedValues = null;
    }

    public CommandArgument(string name, bool required, List<string> allowedValues)
    {
        _name = name;
        _required = required;
        _allowedValues = [.. allowedValues];
    }

    public string GetName()
    {
        return _name;
    }

    public bool IsRequired()
    {
        return _required;
    }

    public List<string>? GetAllowedValues()
    {
        return _allowedValues;
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        if (!_required)
        {
            builder.Append('[');
        }
        if (_allowedValues != null)
        {
            builder.Append('{');
            builder.Append(string.Join(",", _allowedValues));
            builder.Append('}');
        }
        else
        {
            builder.Append(_name.ToUpper());
        }
        if (!_required)
        {
            builder.Append(']');
        }
        return builder.ToString();
    }
}
