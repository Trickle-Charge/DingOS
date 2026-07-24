using System;
using System.IO;

namespace TrickleCharge.DingOS
{
public interface IShellEnvironment
{
    /// <summary>
    /// Gets the output writer for the currently executing async command context.
    /// </summary>
    TextWriter Out { get; }

    /// <summary>
    /// Gets the error writer for the currently executing async command context.
    /// </summary>
    TextWriter Error { get; }

    public DateTime StartTime { get; }

    void RequestClear();
    void RequestQuit();
}
}
