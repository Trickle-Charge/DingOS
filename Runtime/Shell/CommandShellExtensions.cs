using TrickleCharge.Sys.DingOS.Modules;

namespace TrickleCharge.Sys.DingOS.Shell
{
public static class CommandShellExtensions
{
    /// <summary>
    /// Registers the core interactive module suite (Core, Diagnostics, Utility).
    /// </summary>
    public static CommandShell WithInteractiveDefaults(this CommandShell shell)
    {
        return shell
            .WithModule(new CoreShellModule())
            .WithModule(new DiagnosticsModule())
            .WithModule(new UtilityModule());
    }

    /// <summary>
    /// Helper for fluent module registration.
    /// </summary>
    public static CommandShell WithModule(this CommandShell shell, ICommandModule module)
    {
        shell.RegisterModule(module);
        return shell;
    }
}
}
