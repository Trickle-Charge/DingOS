using System;
using System.IO;
using System.Text;

namespace TrickleCharge.Sys.DingOS.Jobs
{
/// <summary>
/// A <see cref="TextWriter"/> adapter that forwards output directly to a <see cref="Job"/>'s log history.
/// </summary>
public class JobTextWriter : TextWriter
{
    private readonly Job _job;
    private readonly StringBuilder _lineBuffer = new();

    public override Encoding Encoding => Encoding.UTF8;

    public JobTextWriter(Job job) => _job = job ?? throw new ArgumentNullException(nameof(job));

    public override void Write(char value)
    {
        if (value == '\n')
        {
            FlushBuffer();
        }
        else if (value != '\r')
        {
            _lineBuffer.Append(value);
        }
    }

    public override void WriteLine(string? value)
    {
        if (_lineBuffer.Length > 0)
        {
            _lineBuffer.Append(value);
            FlushBuffer();
        }
        else if (value != null)
        {
            _job.WriteLine(value);
        }
    }

    public override void Flush() => FlushBuffer();

    private void FlushBuffer()
    {
        if(_lineBuffer.Length <= 0) { return; }

        _job.WriteLine(_lineBuffer.ToString());
        _lineBuffer.Clear();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) { FlushBuffer(); }

        base.Dispose(disposing);
    }
}
}
