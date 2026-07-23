using System.Threading;
using System.Threading.Tasks;

using TrickleCharge.Sys.DingOS.Devices;
using TrickleCharge.Sys.DingOS.Networking;
using TrickleCharge.Sys.DingOS.Shell;

namespace TrickleCharge.Sys.DingOS.Terminal
{
public class TerminalSession<T> where T : ITerminal, new()
{
    public readonly T Terminal = new();
    private readonly ShellContextManager _contextManager;

    public TerminalSession(IDevice device)
    {
        _contextManager = new ShellContextManager(Terminal);

        ShellContext rootContext = device.RequestShell();

        rootContext.CommandShell.RegisterModule(
            new NetworkModule(_contextManager, device.NetworkDirectory)
        );

        _contextManager.PushContext(rootContext);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        Terminal.WriteLine($"Welcome to {SystemInfo.VersionString}");
        Terminal.WriteLine("Type 'help' for available commands or 'exit' to quit.\n");

        await using TerminalTextWriter stdOut = new(Terminal, isError: false);
        await using TerminalTextWriter stdErr = new(Terminal, isError: true);

        while (_contextManager.CurrentContext != null && !cancellationToken.IsCancellationRequested)
        {
            Terminal.Write(_contextManager.ActivePrompt);
            string input = await Terminal.ReadLineAsync(cancellationToken);

            await _contextManager.CurrentContext.ProcessInputAsync(
                input,
                stdOut,
                stdErr,
                cancellationToken
            );
        }
    }
}
}