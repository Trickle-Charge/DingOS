using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.Sys.DingOS.Terminal
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
        Console.ForegroundColor = ConsoleColor.Red;
        _error.WriteLine($"[Error]: {text}");
        Console.ResetColor();
    }

    /// <inheritdoc />
    public string ReadLine() => Console.ReadLine() ?? string.Empty;

    /// <inheritdoc />
    public async Task<string> ReadLineAsync(CancellationToken cancellationToken = default)
    {
        string? input = await Console.In.ReadLineAsync();
        return input ?? string.Empty;
    }

    /// <inheritdoc />
    public void Clear() => Console.Clear();
}
}
