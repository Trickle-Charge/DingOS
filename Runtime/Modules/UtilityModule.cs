namespace TrickleCharge.Sys.DingOS.Modules
{
public class UtilityModule : ICommandModule
{
    public void Register(CommandShell shell)
    {
        shell.RegisterCommand(new[]
        {
            SystemModule.Echo(shell)
        });
    }
}
}
