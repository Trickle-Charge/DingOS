namespace TrickleCharge.Sys.DingOS.Shell
{
using System;
public class ShellContext : IShellContext, IDisposable
{
    public string Name { get; }
    public string Prompt { get; }

    /// <inheritdoc />
    public event Action? ClearRequested;

    /// <inheritdoc />
    public event Action? QuitRequested;

    private readonly CommandShell _commandShell;
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
        _commandShell = commandShell;
        _onEnter = onEnter;
        _onExit = onExit;

        _commandShell.ClearRequested += OnShellClearRequested;
        _commandShell.QuitRequested += OnShellQuitRequested;
    }

    private void OnShellClearRequested() => ClearRequested?.Invoke();
    private void OnShellQuitRequested() => QuitRequested?.Invoke();

    public ShellResult ProcessInput(string input) => _commandShell.Execute(input);

    public void Activate(ITerminal terminal) => _onEnter?.Invoke(terminal);

    public void Deactivate(ITerminal terminal) => _onExit?.Invoke(terminal);

    public void Dispose()
    {
        _commandShell.ClearRequested -= OnShellClearRequested;
        _commandShell.QuitRequested -= OnShellQuitRequested;
    }
}
}