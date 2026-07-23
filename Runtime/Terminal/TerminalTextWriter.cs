using System.IO;
using System.Text;

namespace TrickleCharge.Sys.DingOS.Terminal
{
public class TerminalTextWriter : TextWriter
{
    private readonly ITerminal _terminal;
    private readonly bool _isError;
    private readonly StringBuilder _lineBuffer = new();

    public override Encoding Encoding => Encoding.UTF8;

    public TerminalTextWriter(ITerminal terminal, bool isError = false)
    {
        _terminal = terminal;
        _isError = isError;
    }

    public override void Write(char value)
    {
        if (value == '\n')
        {
            FlushBuffer();
        }
        else if (value != '\r')
        {
            _lineBuffer.Append(value);
        }
    }

    public override void Write(string? value)
    {
        if (string.IsNullOrEmpty(value)) { return; }

        foreach (char c in value) { Write(c); }
    }

    public override void WriteLine(string? value)
    {
        Write(value);
        FlushBuffer();
    }

    public override void Flush()
    {
        if (_lineBuffer.Length > 0) { FlushBuffer(); }
    }

    private void FlushBuffer()
    {
        string line = _lineBuffer.ToString();
        _lineBuffer.Clear();

        if (_isError && !string.IsNullOrWhiteSpace(line))
        {
            _terminal.WriteError(line);
            return;
        }

        _terminal.WriteLine(line);
    }

    protected override void Dispose(bool disposing)
    {
        Flush();
        base.Dispose(disposing);
    }
}
}
