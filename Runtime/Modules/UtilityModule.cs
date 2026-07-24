using System;
using System.Collections.Generic;
using System.CommandLine;

namespace TrickleCharge.DingOS.Modules
{
public class UtilityModule : ICommandModule<Command>
{
    /// <inheritdoc />
    public IEnumerable<Command> GetCommands(IShellEnvironment environment) { yield return Echo(environment); }

    public static Command Echo(IShellEnvironment environment)
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
            environment.Out.WriteLine(string.Join(" ", words));
        });

        return echoCmd;
    }
}
}
