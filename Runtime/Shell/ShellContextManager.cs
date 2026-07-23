using System;
using System.Collections.Generic;

namespace TrickleCharge.DingOS.Shell
{
public class ShellContextManager : IShellContextStack
{
    private readonly Stack<IShellContext> _contextStack = new();
    private readonly ITerminal _terminal;

    public IShellContext? CurrentContext => _contextStack.Count > 0 ? _contextStack.Peek() : null;

    public string ActivePrompt => CurrentContext?.Prompt ?? "> ";

    public ShellContextManager(ITerminal terminal) => _terminal = terminal;

    public void PushContext(IShellContext newContext)
    {
        if (newContext == null) { throw new ArgumentNullException(nameof(newContext)); }

        if(CurrentContext != null)
        {
            UnbindContextEvents(CurrentContext);
            CurrentContext.Deactivate(_terminal);
        }

        _contextStack.Push(newContext);
        BindContextEvents(newContext);

        newContext.Activate(_terminal);
    }

    public void PopContext()
    {
        if (_contextStack.Count == 0) { return; }

        IShellContext poppedContext = _contextStack.Pop();
        UnbindContextEvents(poppedContext);
        poppedContext.Deactivate(_terminal);

        if (poppedContext is IDisposable disposableContext)
        {
            disposableContext.Dispose();
        }

        if(CurrentContext == null) { return; }

        BindContextEvents(CurrentContext);
        CurrentContext.Activate(_terminal);
    }

    private void BindContextEvents(IShellContext context)
    {
        context.ClearRequested += OnClearRequested;
        context.QuitRequested += OnQuitRequested;
    }

    private void UnbindContextEvents(IShellContext context)
    {
        context.ClearRequested -= OnClearRequested;
        context.QuitRequested -= OnQuitRequested;
    }

    private void OnClearRequested() => _terminal.Clear();

    private void OnQuitRequested() => PopContext();
}
}
