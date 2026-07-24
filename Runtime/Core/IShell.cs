using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.DingOS.Core
{
public interface IShell
{
    /// <summary>
    /// Signals when a command or module requests the screen to be cleared.
    /// </summary>
    event Action? ClearRequested;

    /// <summary>
    /// Signals when a command or module requests the shell to quit.
    /// </summary>
    event Action? QuitRequested;

    Task<ShellResult> ExecuteAsync(
        string commandLine,
        TextWriter? outputWriter = null,
        TextWriter? errorWriter = null,
        CancellationToken cancellationToken = default);
}
}
