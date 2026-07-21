using System;
using System.CommandLine;

namespace TrickleCharge.Sys.DingOS.Modules
{
public class SystemModule : ICommandModule
{
    /// <inheritdoc />
    public void Register(CommandShell shell)
    {
        shell.RegisterCommand( new []
        {
            Exit(shell),
            Help(shell),
            Clear(shell),
            SysInfo(shell),
            Echo(shell),
            UpTime(shell)
        });
    }

    public static Command Exit(CommandShell shell)
    {
        Command exitCmd = new("exit", "Exits the command shell.")
        {
            Aliases = { "quit" }
        };

        exitCmd.SetAction(_ => shell.RequestQuit());

        return exitCmd;
    }

    public static Command Help(CommandShell shell)
    {
        Command helpCmd = new("help", "Displays help information.");

        helpCmd.SetAction(_ =>
        {
            InvocationConfiguration config = new()
            {
                Output = shell.Out,
                Error = shell.Error
            };

            shell.Parse("--help").Invoke(config);
        });

        return helpCmd;
    }

    public static Command Clear(CommandShell shell)
    {
        Command clearCmd = new("clear", "Clears the terminal screen buffer.")
        {
            Aliases = { "clr" }
        };

        clearCmd.SetAction(_ => shell.RequestClear());

        return clearCmd;
    }

    public static Command SysInfo(CommandShell shell)
    {
        Command sysInfoCmd = new("sysinfo", "Displays system information.")
        {
            Aliases = { "ver" }
        };

        sysInfoCmd.SetAction(_ => shell.Out.WriteLine(System.VersionString));

        return sysInfoCmd;
    }

    public static Command Echo(CommandShell shell)
    {
        Argument<string[]> textArg = new("text")
        {
            Description = "Text to print to the output buffer.",
            Arity = ArgumentArity.ZeroOrMore
        };

        Command echoCmd = new("echo", "Prints text back to the output buffer.")
        {
            textArg
        };

        echoCmd.SetAction(parseResult =>
        {
            string[] words = parseResult.GetValue(textArg) ?? Array.Empty<string>();
            shell.Out.WriteLine(string.Join(" ", words));
        });

        return echoCmd;
    }

    public static Command UpTime(CommandShell shell)
    {
        Command upTimeCmd = new("uptime", "Displays the system uptime.");

        upTimeCmd.SetAction(_ => shell.Out.WriteLine(shell.Uptime));

        return upTimeCmd;
    }

    // public static Command History(CommandShell shell)
    // {
    //     Command historyCmd = new("history", "Displays the command history.");
    //
    //     historyCmd.SetAction(_ => shell.Out.WriteLine(shell.History));
    //
    //     return historyCmd;
    // }
}
}
