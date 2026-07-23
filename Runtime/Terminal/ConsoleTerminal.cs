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
        Task<string?> readTask = Task.Run<string?>(static () => Console.ReadLine(), cancellationToken);
        Task completedTask = await Task.WhenAny(readTask, Task.Delay(Timeout.Infinite, cancellationToken));

        if (completedTask != readTask)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        return await readTask ?? string.Empty;
    }

    /// <inheritdoc />
    public void Clear() => Console.Clear();
}
}
