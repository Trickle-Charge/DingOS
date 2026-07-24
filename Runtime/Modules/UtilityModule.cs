using System;
using System.CommandLine;

namespace TrickleCharge.DingOS.Modules
{
public class UtilityModule : ICommandModule<CommandShell>
{
    /// <inheritdoc />
    public void Register(CommandShell shell)
    {
        shell.RegisterCommand(new[]
        {
            Echo(shell)
        });

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
}
}
