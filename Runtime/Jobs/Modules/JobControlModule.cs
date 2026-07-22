using System.CommandLine;

namespace TrickleCharge.Sys.DingOS.Jobs.Modules
{
    public class JobControlModule : ICommandModule
    {
        private readonly IJobManager _jobManager;

        public JobControlModule(IJobManager jobManager)
        {
            _jobManager = jobManager;
        }

        public void Register(CommandShell shell)
        {
            shell.RegisterCommand(new[]
            {
                Ps(shell, _jobManager),
                Kill(shell, _jobManager),
                Logs(shell, _jobManager)
            });
        }

        private static readonly Argument<int> s_pidArg = new("pid") { Description = "Process ID." };

        // 'ps' command - List processes
        public static Command Ps(CommandShell shell, IJobManager jobManager)
        {
            Command psCmd = new("ps", "List running jobs.");
            psCmd.SetAction(_ =>
            {
                shell.Out.WriteLine("PID\tNAME\t\tSTATUS");
                shell.Out.WriteLine("---\t----\t\t------");
                foreach (Job? job in jobManager.ActiveJobs)
                {
                    shell.Out.WriteLine($"{job.Pid}\t{job.Name}\t\t{job.Status}");
                }
            });

            return psCmd;
        }

        // 'kill' command - Terminate a process
        public static Command Kill(CommandShell shell, IJobManager jobManager)
        {
            Command killCmd = new("kill", "Terminate a running process.") { s_pidArg };
            killCmd.SetAction(parseResult =>
            {
                int pid = parseResult.GetValue(s_pidArg);
                if (jobManager.KillJob(pid))
                {
                    shell.Out.WriteLine($"Signal sent to PID {pid}.");
                }
                else
                {
                    shell.Error.WriteLine($"PID {pid} not found.");
                }
            });

            return killCmd;
        }

        // 'logs' command - View output buffer of a background process
        public static Command Logs(CommandShell shell, IJobManager jobManager)
        {
            Command logsCmd = new("logs", "View recent output of a process.") { s_pidArg };
            logsCmd.SetAction(parseResult =>
            {
                int pid = parseResult.GetValue(s_pidArg);
                Job? job = jobManager.GetJob(pid);
                if (job != null)
                {
                    foreach (string line in job.LogHistory)
                    {
                        shell.Out.WriteLine(line);
                    }
                }
                else
                {
                    shell.Error.WriteLine($"PID {pid} not found.");
                }
            });

            return logsCmd;
        }
    }
}