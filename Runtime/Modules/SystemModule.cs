using System;
using System.Collections.Generic;
using System.CommandLine;

using TrickleCharge.DingOS.Core;

namespace TrickleCharge.DingOS.Modules
{
public class SystemModule : ICommandModule<Command>
{
    private readonly IShell _shell;

    public SystemModule(IShell shell)
    {
        _shell = shell ?? throw new ArgumentNullException(nameof(shell));
    }

    /// <inheritdoc />
    public IEnumerable<Command> GetCommands(IShellEnvironment environment)
    {
        foreach (Command cmd in new CoreShellModule(_shell).GetCommands(environment)) { yield return cmd; }
        foreach (Command cmd in new DiagnosticsModule().GetCommands(environment)) { yield return cmd; }
        foreach (Command cmd in new UtilityModule().GetCommands(environment)) { yield return cmd; }
    }
}
}