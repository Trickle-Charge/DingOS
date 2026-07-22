using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using TrickleCharge.Sys.DingOS.Jobs;

namespace TrickleCharge.Sys.DingOS
{
public interface IJobManager
{
    IEnumerable<Job> ActiveJobs { get; }
    Job StartJob(string name, Func<Job, CancellationToken, Task> work);
    bool KillJob(int pid);
    Job? GetJob(int pid);
}
}
