using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.Sys.DingOS.Terminal
{

public sealed class TerminalHost
{
    private readonly ITerminal _terminal;

    private IShellContextStack ContextStack { get; }

    public TerminalHost(ITerminal terminal, IShellContextStack contextStack)
    {
        _terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
        ContextStack = contextStack ?? throw new ArgumentNullException(nameof(contextStack));
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _terminal.WriteLine($"Welcome to {SystemInfo.VersionString}");
        _terminal.WriteLine("Type 'help' for available commands or 'exit' to quit.\n");

        await using TerminalTextWriter stdOut = new(_terminal.Write, _terminal.WriteLine);
        await using TerminalTextWriter stdErr = new(_terminal.WriteError, _terminal.WriteErrorLine);

        while(ContextStack.CurrentContext != null && ! cancellationToken.IsCancellationRequested)
        {
            _terminal.Write(ContextStack.ActivePrompt);
            string input = _terminal.ReadLine();

            await ContextStack.CurrentContext.ProcessInputAsync(
                input,
                stdOut,
                stdErr,
                cancellationToken
            );
        }
    }
}
}