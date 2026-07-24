using TrickleCharge.DingOS.Modules;

namespace TrickleCharge.DingOS.Shell
{
public static class CommandShellExtensions
{
    /// <summary>
    /// Registers the core interactive module suite (Core, Diagnostics, Utility).
    /// </summary>
    public static CommandShell WithInteractiveDefaults(this CommandShell shell) => shell.WithModule(new SystemModule());

    /// <summary>
    /// Helper for fluent module registration.
    /// </summary>
    public static CommandShell WithModule(this CommandShell shell, ICommandModule<CommandShell> module)
    {
        shell.RegisterModule(module);
        return shell;
    }
}
}
