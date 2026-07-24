namespace TrickleCharge.DingOS.Modules
{
public class SystemModule : ICommandModule<CommandShell>
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
