using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.DingOS.Terminal
{
public class TerminalTextWriter : TextWriter
{
    private readonly Action<string> _write;
    private readonly Action<string> _writeLine;

    public override Encoding Encoding => Encoding.UTF8;

    public TerminalTextWriter(Action<string> write, Action<string> writeLine)
    {
        _write = write ?? throw new ArgumentNullException(nameof(write));
        _writeLine = writeLine ?? throw new ArgumentNullException(nameof(writeLine));
    }

    // --- Core Synchronous Overrides ---

    public override void Write(char value) => Write(value.ToString());

    public override void Write(char[] buffer, int index, int count)
    {
        if (buffer == null || count <= 0) { return; }

        Write(new string(buffer, index, count));
    }

    public override void Write(string? value)
    {
        if(string.IsNullOrEmpty(value)) { return; }
        _write(value);
    }

    public override void WriteLine(string? value) => _writeLine(value ?? string.Empty);

    // --- Asynchronous Overrides (Prevents Base TextWriter ThreadPool Offloading) ---

    // Strings
    public override Task WriteAsync(string? value)
    {
        Write(value);
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(string? value)
    {
        WriteLine(value);
        return Task.CompletedTask;
    }

    // Chars & Char Arrays
    public override Task WriteAsync(char value)
    {
        Write(value);
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(char value)
    {
        WriteLine(value);
        return Task.CompletedTask;
    }

    public override Task WriteAsync(char[] buffer, int index, int count)
    {
        Write(buffer, index, count);
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(char[] buffer, int index, int count)
    {
        WriteLine(buffer, index, count);
        return Task.CompletedTask;
    }

    // Memory / Span Buffers
    public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        Write(buffer.ToString());
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        WriteLine(buffer.ToString());
        return Task.CompletedTask;
    }
}
}
