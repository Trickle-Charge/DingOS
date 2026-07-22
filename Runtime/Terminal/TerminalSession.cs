using TrickleCharge.Sys.DingOS.Devices;
using TrickleCharge.Sys.DingOS.Networking.Modules;
using TrickleCharge.Sys.DingOS.Shell;

namespace TrickleCharge.Sys.DingOS.Terminal
{
public class TerminalSession<T> where T : ITerminal, new()
{
    public readonly T Terminal = new();

    public TerminalSession(IDevice device)
    {
        ShellContextManager contextManager = new(Terminal);

        ShellContext rootContext = device.RequestShell();

        rootContext.CommandShell.RegisterModule(
            new NetworkModule(contextManager, device.NetworkDirectory, device.JobManager)
        );

        contextManager.PushContext(rootContext);

        Terminal.WriteLine($"Welcome to {SystemInfo.VersionString}");
        Terminal.WriteLine("Type 'help' for available commands or 'exit' to quit.\n");

        while (contextManager.CurrentContext != null)
        {
            Terminal.Write(contextManager.ActivePrompt);
            string input = Terminal.ReadLine();

            ShellResult result = contextManager.CurrentContext.ProcessInput(input);

            if (!string.IsNullOrEmpty(result.Output))
            {
                Terminal.WriteLine(result.Output);
            }

            if (!string.IsNullOrEmpty(result.Error))
            {
                Terminal.WriteError(result.Error);
            }
        }
    }
}
}
