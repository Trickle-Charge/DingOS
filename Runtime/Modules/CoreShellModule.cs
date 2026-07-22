namespace TrickleCharge.Sys.DingOS.Modules
{
public class CoreShellModule : ICommandModule
{
    public void Register(CommandShell shell)
    {
        shell.RegisterCommand(new[]
        {
            SystemModule.Exit(shell),
            SystemModule.Clear(shell),
            SystemModule.Help(shell)
        });
    }
}
}
