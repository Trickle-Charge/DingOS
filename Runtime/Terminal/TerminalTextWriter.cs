using System;
using System.IO;
using System.Text;

namespace TrickleCharge.Sys.DingOS.Terminal
{
public class TerminalTextWriter : TextWriter
{
    private readonly Action<string> _write;
    private readonly Action<string> _writeLine;

    public override Encoding Encoding => Encoding.UTF8;

    public TerminalTextWriter(Action<string> write, Action<string> writeLine)
    {
        _write = write;
        _writeLine = writeLine;
    }

    public override void Write(char value) => _write(value.ToString());

    public override void Write(string? value)
    {
        if (!string.IsNullOrEmpty(value)) { _write(value); }
    }

    public override void WriteLine(string? value) => _writeLine(value ?? string.Empty);
}
}
