namespace TrickleCharge.Sys.DingOS.Modules
{
public class SystemModule : ICommandModule
{
    /// <inheritdoc />
    public void Register(CommandShell shell)
    {
        shell.RegisterModule(new CoreShellModule());
        shell.RegisterModule(new DiagnosticsModule());
        shell.RegisterModule(new UtilityModule());
    }
}
}
