using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.Sys.DingOS.Jobs
{
public sealed class JobManager : IJobManager
{
    private readonly ConcurrentDictionary<int, Job> _jobs = new();
    private int _nextPid = 1;

    public IEnumerable<Job> ActiveJobs => _jobs.Values;

    public Job StartJob(string name, Func<Job, CancellationToken, Task> work)
    {
        int pid = Interlocked.Increment(ref _nextPid);
        Job job = new(pid, name);

        _jobs[pid] = job;

        Task runningTask = Task.Run(async () =>
        {
            try
            {
                await work(job, job.CancellationToken);
            }
            catch (OperationCanceledException)
            {
                job.WriteLine("Process terminated.");
            }
            catch (Exception ex)
            {
                job.WriteError($"Process crashed: {ex.Message}");
            }
        }, job.CancellationToken);

        job.AttachTask(runningTask);
        return job;
    }

    public bool KillJob(int pid)
    {
        if(! _jobs.TryGetValue(pid, out Job? job)) { return false; }

        job.Kill();
        return true;
    }

    public Job? GetJob(int pid)
    {
        _jobs.TryGetValue(pid, out Job? job);
        return job;
    }
}
}
