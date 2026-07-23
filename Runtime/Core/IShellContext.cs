using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.DingOS
{
public interface IShellContext
{
    string Name { get; }
    string Prompt { get; }
    CommandShell CommandShell { get; }

    event Action? ClearRequested;
    event Action? QuitRequested;

    Task<ShellResult> ProcessInputAsync(
        string input,
        TextWriter? outputWriter = null,
        TextWriter? errorWriter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Called when this context becomes active on top of the stack
    /// </summary>
    void Activate(ITerminal terminal);

    /// <summary>
    /// Called when this context is popped or closed
    /// </summary>
    void Deactivate(ITerminal terminal);
}
}
