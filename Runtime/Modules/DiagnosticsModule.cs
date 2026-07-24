using System;
using System.Collections.Generic;
using System.CommandLine;

using TrickleCharge.DingOS.Core;

namespace TrickleCharge.DingOS.Modules
{
public class DiagnosticsModule : ICommandModule<Command>
{
    /// <inheritdoc />
    public IEnumerable<Command> GetCommands(IShellEnvironment environment)
    {
        yield return SysInfo(environment);
        yield return UpTime(environment);
    }

    public static Command SysInfo(IShellEnvironment environment)
    {
        Command sysInfoCmd = new("sysinfo", "Displays system information.");
        sysInfoCmd.Aliases.Add("ver");

        sysInfoCmd.SetAction(_ => environment.Out.WriteLine(SystemInfo.VersionString));

        return sysInfoCmd;
    }

    public static Command UpTime(IShellEnvironment environment)
    {
        Command upTimeCmd = new("uptime", "Displays the system uptime.");

        upTimeCmd.SetAction(_ => environment.Out.WriteLine(DateTime.UtcNow - environment.StartTime));

        return upTimeCmd;
    }
}
}
