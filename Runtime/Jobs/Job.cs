using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.Sys.DingOS.Jobs
{
public sealed class Job : IDisposable
{
    public int Pid { get; }
    public string Name { get; }
    public JobStatus Status { get; private set; } = JobStatus.Running;

    public DateTime StartTime { get; } = DateTime.UtcNow;

    private readonly CancellationTokenSource _cts = new();
    public CancellationToken CancellationToken => _cts.Token;

    // Circular buffer or thread-safe collection for detached logs
    private readonly ConcurrentQueue<string> _logHistory = new();
    public IEnumerable<string> LogHistory => _logHistory;

    public event Action<string>? OnOutput;
    public event Action<string>? OnError;

    public Task Task { get; private set; } = Task.CompletedTask;

    public Job(int pid, string name)
    {
        Pid = pid;
        Name = name;
    }

    public void AttachTask(Task task)
    {
        Task = task;
        // Automatically update status when the task finishes
        _ = task.ContinueWith(t =>
        {
            if (t.IsCanceled)
            {
                Status = JobStatus.Canceled;
            }
            else if (t.IsFaulted)
            {
                Status = JobStatus.Faulted;
            }
            else
            {
                Status = JobStatus.Completed;
            }
        }, TaskScheduler.Default);
    }

    public void WriteLine(string text)
    {
        _logHistory.Enqueue(text);
        // Keep buffer reasonable (e.g., last 100 lines)
        while (_logHistory.Count > 100) { _logHistory.TryDequeue(out _); }

        OnOutput?.Invoke(text);
    }

    public void WriteError(string text)
    {
        _logHistory.Enqueue($"[Error] {text}");
        OnError?.Invoke(text);
    }

    public void Kill() => _cts.Cancel();

    public void Dispose() => _cts.Dispose();
}
}
