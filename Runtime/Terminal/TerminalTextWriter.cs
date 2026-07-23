using System.IO;
using System.Text;

namespace TrickleCharge.Sys.DingOS.Terminal
{
public class TerminalTextWriter : TextWriter
{
    private readonly ITerminal _terminal;
    private readonly bool _isError;

    public override Encoding Encoding => Encoding.UTF8;

    public TerminalTextWriter(ITerminal terminal, bool isError = false)
    {
        _terminal = terminal;
        _isError = isError;
    }

    public override void WriteLine(string? value)
    {
        if (_isError)
        {
            _terminal.WriteError(value ?? string.Empty);
        }
        else
        {
            _terminal.WriteLine(value ?? string.Empty);
        }
    }

    public override void Write(char value)
    {
        if (_isError)
        {
            _terminal.WriteError(value.ToString());
        }
        else
        {
            _terminal.Write(value.ToString());
        }
    }

    public override void Write(string? value)
    {
        if (string.IsNullOrEmpty(value)) { return; }

        if (_isError)
        {
            _terminal.WriteError(value);
        }
        else
        {
            _terminal.Write(value);
        }
    }
}
}
