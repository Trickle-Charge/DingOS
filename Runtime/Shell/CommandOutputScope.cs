using System;
using System.IO;

namespace TrickleCharge.DingOS.Shell
{
public sealed class CommandOutputScope : IDisposable
{
    public TextWriter OutputWriter { get; }
    public TextWriter ErrorWriter { get; }

    private readonly StringWriter? _capturedOut;
    private readonly StringWriter? _capturedErr;

    public CommandOutputScope(TextWriter? customOut, TextWriter? customErr)
    {
        OutputWriter = customOut ?? (_capturedOut = new StringWriter());
        ErrorWriter = customErr ?? (_capturedErr = new StringWriter());
    }

    public string GetOutputText() => _capturedOut?.ToString().TrimEnd('\r', '\n') ?? string.Empty;
    public string GetErrorText() => _capturedErr?.ToString().TrimEnd('\r', '\n') ?? string.Empty;

    public void Dispose()
    {
        _capturedOut?.Dispose();
        _capturedErr?.Dispose();
    }
}
}
