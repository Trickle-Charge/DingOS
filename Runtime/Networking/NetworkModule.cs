using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.Threading.Tasks;

using TrickleCharge.Sys.DingOS.Devices;
using TrickleCharge.Sys.DingOS.Shell;

namespace TrickleCharge.Sys.DingOS.Networking
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

    public static Command Connect(
        CommandShell shell,
        IShellContextStack contextStack,
        IDeviceDirectory deviceDirectory)
    {
        Argument<string> hostArgument = new("host")
        {
            Description = "Hostname or IP address."
        };

        Command connectCmd = new("connect", "Spawns a sub-shell session for a host.")
        {
            hostArgument
        };

        connectCmd.SetAction(parseResult =>
        {
            string host = parseResult.GetValue(hostArgument) ?? string.Empty;

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
        listCmd.Aliases.Add("ls");

        listCmd.SetAction(_ =>
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
        Argument<string> hostArgument = new("host")
        {
            Description = "Hostname or IP address."
        };

        Command pingCmd = new("ping", "Ping host.") { hostArgument };

        pingCmd.SetAction(async (parseResult, cancellationToken) =>
        {
            string host = parseResult.GetValue(hostArgument) ?? "unknown";
            Stopwatch sw = new();

            for (int i = 1; i <= 5; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                sw.Restart();
                await Task.Delay(100, cancellationToken);
                sw.Stop();

                await shell.Out.WriteLineAsync($"Reply from {host}: bytes=32 time={sw.ElapsedMilliseconds}ms");
            }
        });

        return pingCmd;
    }
}
}
