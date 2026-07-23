using System.CommandLine;

namespace TrickleCharge.Sys.DingOS.Modules
{
public class CoreShellModule : ICommandModule
{
    public void Register(CommandShell shell)
    {
        shell.RegisterCommand(new[]
        {
            Exit(shell),
            Clear(shell),
            Help(shell)
        });
    }

    public static Command Exit(CommandShell shell)
    {
        Command exitCmd = new("exit", "Exits the command shell.");
        exitCmd.Aliases.Add("quit");

        exitCmd.SetAction(_ => shell.RequestQuit());

        return exitCmd;
    }

    public static Command Help(CommandShell shell)
    {
        Command helpCmd = new("help", "Displays help information.");

        helpCmd.SetAction(async (_, cancellationToken) =>
        {
            InvocationConfiguration config = new()
            {
                Output = shell.Out,
                Error = shell.Error
            };

            await shell.Parse("--help").InvokeAsync(config, cancellationToken);
        });

        return helpCmd;
    }

    public static Command Clear(CommandShell shell)
    {
        Command clearCmd = new("clear", "Clears the terminal screen buffer.");
        clearCmd.Aliases.Add("clr");

        clearCmd.SetAction(_ => shell.RequestClear());

        return clearCmd;
    }
}
}
