using System;
using System.CommandLine;

namespace TrickleCharge.DingOS.Modules
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
        Argument<string[]> topicArg = new("topic")
        {
            Description = "Optional command or subcommand to get help for.",
            Arity = ArgumentArity.ZeroOrMore
        };

        Command helpCmd = new("help", "Displays help information.") { topicArg };

        helpCmd.SetAction(async (parseResult, cancellationToken) =>
        {
            string[] targetArgs = parseResult.GetValue(topicArg) ?? Array.Empty<string>();

            if (targetArgs.Length == 0)
            {
                await shell.ExecuteAsync("--help", shell.Out, shell.Error, cancellationToken);
                return;
            }

            string rawQuery = string.Join(" ", targetArgs);
            ParseResult check = shell.Parse(rawQuery);

            // Check if topic is invalid (didn't match a subcommand OR has trailing unmatched tokens)
            if (check.CommandResult.Command == shell || check.UnmatchedTokens.Count > 0)
            {
                await shell.Error.WriteLineAsync($"Unrecognized command or topic '{rawQuery}'.");
                return;
            }

            await shell.ExecuteAsync($"{rawQuery} --help", shell.Out, shell.Error, cancellationToken);
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
