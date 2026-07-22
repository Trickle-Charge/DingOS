using System;
using System.CommandLine;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.Sys.DingOS.Jobs
{
public static class CommandJobExtensions
{
    /// <summary>
    /// Helper to register an async command action that can be executed as a managed Job.
    /// </summary>
    public static void SetAsyncAction(
        this Command command,
        CommandShell shell,
        IJobManager jobManager,
        Func<ParseResult, TextWriter, CancellationToken, Task> action)
    {
        command.SetAction(parseResult =>
        {
            Job job = jobManager.StartJob(command.Name, async (j, ct) =>
            {
                await using JobTextWriter jobWriter = new(j);
                await action(parseResult, jobWriter, ct);
            });

            shell.Out.WriteLine($"[{job.Name} started with PID {job.Pid}]");
        });
    }
}
}
