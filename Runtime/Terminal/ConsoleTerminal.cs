using System;

namespace TrickleCharge.Sys.DingOS.Terminal
{
public class ConsoleTerminal : ITerminal
{
    /// <inheritdoc />
    public void Write(string text) =>  Console.Write(text);

    /// <inheritdoc />
    public void WriteLine(string text) => Console.WriteLine(text);

    /// <inheritdoc />
    public void WriteError(string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Error]: {text}");
        Console.ResetColor();
    }

    /// <inheritdoc />
    public string ReadLine() => Console.ReadLine() ?? string.Empty;

    /// <inheritdoc />
    public void Clear() => Console.Clear();
}
}
