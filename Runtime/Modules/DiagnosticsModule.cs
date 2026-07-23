using System.CommandLine;

namespace TrickleCharge.DingOS.Modules
{
public class DiagnosticsModule : ICommandModule
{
    public void Register(CommandShell shell)
    {
        shell.RegisterCommand(new[]
        {
            SysInfo(shell),
            UpTime(shell)
        });
    }

    public static Command SysInfo(CommandShell shell)
    {
        Command sysInfoCmd = new("sysinfo", "Displays system information.");
        sysInfoCmd.Aliases.Add("ver");

        sysInfoCmd.SetAction(_ => shell.Out.WriteLine(SystemInfo.VersionString));

        return sysInfoCmd;
    }

    public static Command UpTime(CommandShell shell)
    {
        Command upTimeCmd = new("uptime", "Displays the system uptime.");

        upTimeCmd.SetAction(_ => shell.Out.WriteLine(shell.Uptime));

        return upTimeCmd;
    }
}
}
