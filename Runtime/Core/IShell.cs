using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.DingOS
{
public interface IShell
{
    /// <summary>
    /// Gets the output writer for the currently executing async command context.
    /// </summary>
    TextWriter Out { get; }

    /// <summary>
    /// Gets the error writer for the currently executing async command context.
    /// </summary>
    TextWriter Error { get; }

    /// <summary>
    /// Signals when a command or module requests the screen to be cleared.
    /// </summary>
    event Action? ClearRequested;

    /// <summary>
    /// Signals when a command or module requests the shell to quit.
    /// </summary>
    event Action? QuitRequested;

    void RequestClear();
    void RequestQuit();

    Task<ShellResult> ExecuteAsync(
        string commandLine,
        TextWriter? outputWriter = null,
        TextWriter? errorWriter = null,
        CancellationToken cancellationToken = default);
}
}
