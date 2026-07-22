using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;

using TrickleCharge.Sys.DingOS.Devices;
using TrickleCharge.Sys.DingOS.Shell;

namespace TrickleCharge.Sys.DingOS.Networking.Modules
{
public class NetworkModule : ICommandModule
{
    private readonly IShellContextStack _contextStack;

    private readonly IDeviceDirectory _deviceDirectory;
    public NetworkModule(IShellContextStack contextStack, IDeviceDirectory deviceDirectory)
    {
        _deviceDirectory = deviceDirectory;
        _contextStack = contextStack
                        ?? throw new ArgumentNullException(nameof(contextStack));
    }

    public void Register(CommandShell shell)
    {
        Command netCommand = new("net", "Network tools.");

        netCommand.Subcommands.Add(Connect(shell, _contextStack, _deviceDirectory));
        netCommand.Subcommands.Add(List(shell, _deviceDirectory));
        netCommand.Subcommands.Add(PingAsync(shell));

        shell.RegisterCommand(netCommand);
    }

    private static readonly Argument<string> s_hostArgument = new("host") { Description = "Hostname or IP address." };

    public static Command Connect(
        CommandShell shell,
        IShellContextStack contextStack,
        IDeviceDirectory deviceDirectory)
    {
        Command connectCmd = new("connect", "Spawns a sub-shell session for a host.")
        {
            s_hostArgument
        };

        connectCmd.SetAction(parseResult =>
        {
            string host = parseResult.GetValue(s_hostArgument) ?? string.Empty;

            if(deviceDirectory.TryGetValue(host, out IDevice targetDevice))
            {
                shell.Out.WriteLine($"Connecting to {host}...");

                ShellContext remoteContext = targetDevice.RequestShell();

                remoteContext.CommandShell.RegisterModule(
                    new NetworkModule(contextStack, targetDevice.NetworkDirectory)
                );

                contextStack.PushContext(remoteContext);
                return;
            }

            shell.Error.WriteLine($"Failed to connect to {host}.");
        });

        return connectCmd;
    }

    public static Command List(CommandShell shell, IDeviceDirectory deviceDirectory)
    {
        Command listCmd = new("list", "List all available devices.");

        listCmd.SetAction(parseResult =>
        {
            foreach (KeyValuePair<string, IDevice> device in deviceDirectory)
            {
                shell.Out.WriteLine($"- {device.Key}");
            }
        });

        return listCmd;
    }

    public static Command PingAsync(CommandShell shell)
    {
        Command pingCmd = new("ping", "Ping host.") { s_hostArgument };

        pingCmd.SetAction(async (parseResult, cancellationToken) =>
        {
            string? host = parseResult.GetValue(s_hostArgument);

            for (int i = 1; i <= 5; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(1000, cancellationToken);
                await shell.Out.WriteLineAsync($"Reply from {host}: bytes=32 time={i}ms");
            }
        });

        return pingCmd;
    }

    public static Command Ping(CommandShell shell)
    {
        const int timeout = 1000;

        Command pingCmd = new("ping", "Ping host.") { s_hostArgument };

        pingCmd.SetAction(parseResult =>
        {
            string? host = parseResult.GetValue(s_hostArgument);

            for (int i = 1; i <= 5; i++)
            {
                Thread.Sleep(timeout);
                shell.Out.WriteLine($"Reply from {host}: bytes=32 time={DateTime.UtcNow.Millisecond}ms");
            }
        });

        return pingCmd;
    }
}
}
