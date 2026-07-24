using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.DingOS
{
public interface ITerminalHost
{
    IShellContextStack ContextStack { get; }

    /// <summary>
    /// Runs a continuous blocking ReadLine loop for OS console environments. (Pull API - CLI Playground)
    /// </summary>
    Task RunConsoleLoopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a single input command against the current active context. (Push API - Unity / UI / Web)
    /// </summary>
    Task<ShellResult> ExecuteAsync(string input, CancellationToken cancellationToken = default);
}
}