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
        Option<string> timespanFormatOption = new("--format", "-f")
        {
            Description = "Custom TimeSpan.ToString format string.",
            HelpName = "format-specifier",
            DefaultValueFactory = static _ => @"dd\.hh\:mm\:ss"
        };

        Command upTimeCmd = new("uptime", "Displays the system uptime.") { timespanFormatOption };

        upTimeCmd.SetAction(parseResult =>
        {
            string? format = parseResult.GetValue(timespanFormatOption);
            TimeSpan uptime = DateTime.UtcNow - environment.StartTime;

            try
            {
                environment.Out.WriteLine(uptime.ToString(format));
            }
            catch (FormatException ex)
            {
                environment.Error.WriteLine($"Invalid format specifier '{format}'.");
                environment.Error.WriteLine(ex);
            }
        });

        return upTimeCmd;
    }
}
}
