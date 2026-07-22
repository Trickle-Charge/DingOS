namespace TrickleCharge.Sys.DingOS.Modules
{
public class DiagnosticsModule : ICommandModule
{
    public void Register(CommandShell shell)
    {
        shell.RegisterCommand(new[]
        {
            SystemModule.SysInfo(shell),
            SystemModule.UpTime(shell)
        });
    }
}
}
