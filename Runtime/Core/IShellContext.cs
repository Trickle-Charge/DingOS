using System;

namespace TrickleCharge.Sys.DingOS
{
public interface IShellContext
{
    string Name { get; }
    string Prompt { get; }
    CommandShell CommandShell { get; }

    event Action? ClearRequested;
    event Action? QuitRequested;

    ShellResult ProcessInput(string input);

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
