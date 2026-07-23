using System;
using System.IO;

namespace TrickleCharge.DingOS.Terminal
{
public class ConsoleTerminal : ITerminal
{
    private readonly TextWriter _out = Console.Out;
    private readonly TextWriter _error = Console.Error;

    /// <inheritdoc />
    public void Write(string text) => _out.Write(text);

    /// <inheritdoc />
    public void WriteLine(string text) => _out.WriteLine(text);

    /// <inheritdoc />
    public void WriteError(string text)
    {
        if (string.IsNullOrEmpty(text)) { return; }

        Console.ForegroundColor = ConsoleColor.Red;
        _error.Write(text);
        Console.ResetColor();
    }

    /// <inheritdoc />
    public void WriteErrorLine(string text)
    {
        WriteError(text);
        _error.WriteLine();
    }

    /// <inheritdoc />
    public string ReadLine() => Console.ReadLine() ?? string.Empty;

    /// <inheritdoc />
    public void Clear() => Console.Clear();
}
}
