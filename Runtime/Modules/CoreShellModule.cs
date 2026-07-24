using System;
using System.Collections.Generic;
using System.CommandLine;

namespace TrickleCharge.DingOS.Modules
{
public class CoreShellModule : ICommandModule<Command>
{
    private readonly IShell _shell;

    public CoreShellModule(IShell shell)
    {
        _shell = shell ?? throw new ArgumentNullException(nameof(shell));
    }

    /// <inheritdoc />
    public IEnumerable<Command> GetCommands(IShellEnvironment environment)
    {
        yield return Exit(environment);
        yield return Clear(environment);
        yield return Help(environment, _shell);
    }

    public static Command Exit(IShellEnvironment environment)
    {
        Command exitCmd = new("exit", "Exits the command shell.");
        exitCmd.Aliases.Add("quit");

        exitCmd.SetAction(_ => environment.RequestQuit());

        return exitCmd;
    }

    public static Command Help(IShellEnvironment environment, IShell shell)
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
                await shell.ExecuteAsync(
                    "--help",
                    environment.Out,
                    environment.Error,
                    cancellationToken
                );

                return;
            }

            Command rootCommand = parseResult.RootCommandResult.Command;

            string rawQuery = string.Join(" ", targetArgs);
            ParseResult check = rootCommand.Parse(rawQuery);

            // Check if topic is invalid (didn't match a subcommand OR has trailing unmatched tokens)
            if (check.CommandResult.Command == rootCommand || check.UnmatchedTokens.Count > 0)
            {
                await environment.Error.WriteLineAsync($"Unrecognized command or topic '{rawQuery}'.");
                return;
            }

            await shell.ExecuteAsync(
                $"{rawQuery} --help",
                environment.Out,
                environment.Error,
                cancellationToken
            );
        });

        return helpCmd;
    }

    public static Command Clear(IShellEnvironment environment)
    {
        Command clearCmd = new("clear", "Clears the terminal screen buffer.");
        clearCmd.Aliases.Add("clr");

        clearCmd.SetAction(_ => environment.RequestClear());

        return clearCmd;
    }
}
}
