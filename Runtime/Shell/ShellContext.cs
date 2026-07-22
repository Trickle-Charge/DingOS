namespace TrickleCharge.Sys.DingOS.Shell
{
using System;
public class ShellContext : IShellContext, IDisposable
{
    public string Name { get; }
    public string Prompt { get; }
    public CommandShell CommandShell { get; }

    /// <inheritdoc />
    public event Action? ClearRequested;

    /// <inheritdoc />
    public event Action? QuitRequested;

    private readonly Action<ITerminal>? _onEnter;
    private readonly Action<ITerminal>? _onExit;

    public ShellContext(
        string name,
        string prompt,
        CommandShell commandShell,
        Action<ITerminal>? onEnter = null,
        Action<ITerminal>? onExit = null)
    {
        Name = name;
        Prompt = prompt;
        CommandShell = commandShell;
        _onEnter = onEnter;
        _onExit = onExit;

        CommandShell.ClearRequested += OnShellClearRequested;
        CommandShell.QuitRequested += OnShellQuitRequested;
    }

    private void OnShellClearRequested() => ClearRequested?.Invoke();
    private void OnShellQuitRequested() => QuitRequested?.Invoke();

    public ShellResult ProcessInput(string input) => CommandShell.Execute(input);

    public void Activate(ITerminal terminal) => _onEnter?.Invoke(terminal);

    public void Deactivate(ITerminal terminal) => _onExit?.Invoke(terminal);

    public void Dispose()
    {
        CommandShell.ClearRequested -= OnShellClearRequested;
        CommandShell.QuitRequested -= OnShellQuitRequested;
    }
}
}