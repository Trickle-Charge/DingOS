using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Help;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.Sys.DingOS
{
public sealed class CommandShell : Command
{
    /// <summary>
    /// Gets the output writer for the currently executing command.
    /// </summary>
    public TextWriter Out { get; private set; } = TextWriter.Null;

    /// <summary>
    /// Gets the error writer for the currently executing command.
    /// </summary>
    public TextWriter Error { get; private set; } = TextWriter.Null;

    /// <summary>
    /// Signals when a command or module requests the screen to be cleared.
    /// </summary>
    public event Action? ClearRequested;

    /// <summary>
    /// Signals when a command or module requests the shell to quit.
    /// </summary>
    public event Action? QuitRequested;

    public DateTime StartTime { get; } = DateTime.UtcNow;
    public TimeSpan Uptime => DateTime.UtcNow - StartTime;

    public CommandShell(string name = SystemInfo.ApplicationName, string description = SystemInfo.VersionString)
        : base(name, description)
    {
        Options.Add(new HelpOption());
    }

    public void RegisterModule(ICommandModule module)
    {
        if (module == null) { throw new ArgumentNullException(nameof(module)); }

        module.Register(this);
    }

    public void RegisterCommand(Command command)
    {
        if (command == null) { throw new ArgumentNullException(nameof(command)); }

        Subcommands.Add(command);
    }

    public void RegisterCommand(IEnumerable<Command> commands)
    {
        foreach (Command command in commands) { RegisterCommand(command); }
    }

    public void RequestClear() => ClearRequested?.Invoke();

    public void RequestQuit() => QuitRequested?.Invoke();

    public async Task<ShellResult> ExecuteAsync(
        string commandLine,
        TextWriter? outputWriter = null,
        TextWriter? errorWriter = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(commandLine))
        {
            return new ShellResult(0, string.Empty, string.Empty);
        }

        StringBuilder outputBuffer = new();
        StringBuilder errorBuffer = new();

        await using TextWriter stdOutWriter = outputWriter ?? new StringWriter(outputBuffer);
        await using TextWriter stdErrWriter = errorWriter ?? new StringWriter(errorBuffer);

        Out = stdOutWriter;
        Error = stdErrWriter;

        InvocationConfiguration config = new()
        {
            Output = stdOutWriter,
            Error = stdErrWriter
        };

        int exitCode;

        try
        {
            exitCode = await Parse(commandLine).InvokeAsync(config, cancellationToken);
        }
        finally
        {
            Out = TextWriter.Null;
            Error = TextWriter.Null;
        }

        return new ShellResult(
            exitCode,
            outputBuffer.ToString().TrimEnd(),
            errorBuffer.ToString().TrimEnd()
        );
    }
}
}
