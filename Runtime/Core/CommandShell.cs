using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.Sys.DingOS
{
public sealed class CommandShell : Command
{
    private readonly AsyncLocal<TextWriter?> _currentOut = new();
    private readonly AsyncLocal<TextWriter?> _currentErr = new();

    /// <summary>
    /// Gets the output writer for the currently executing async command context.
    /// </summary>
    public TextWriter Out => _currentOut.Value ?? TextWriter.Null;

    /// <summary>
    /// Gets the error writer for the currently executing async command context.
    /// </summary>
    public TextWriter Error => _currentErr.Value ?? TextWriter.Null;

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
        if (string.IsNullOrWhiteSpace(commandLine)) { return ShellResult.Empty; }

        using CommandOutputScope outputScope = new(outputWriter, errorWriter);

        _currentOut.Value = outputScope.OutputWriter;
        _currentErr.Value = outputScope.ErrorWriter;

        int exitCode;

        try
        {
            if(TryParseCommand(commandLine, outputScope.ErrorWriter, out ParseResult parseResult))
            {
                InvocationConfiguration config = new()
                {
                    Output = outputScope.OutputWriter,
                    Error = outputScope.ErrorWriter
                };

                exitCode = await parseResult.InvokeAsync(config, cancellationToken);
            }
            else { exitCode = 1; }
        }
        finally
        {
            _currentOut.Value = TextWriter.Null;
            _currentErr.Value = TextWriter.Null;
        }

        return new ShellResult(
            exitCode,
            outputScope.GetOutputText(),
            outputScope.GetErrorText()
        );
    }

    private bool TryParseCommand(string command, TextWriter errorWriter, out ParseResult parseResult)
    {
        parseResult = Parse(command);

        if(parseResult.Errors.Count <= 0) { return true; }

        foreach (ParseError error in parseResult.Errors)
        {
            errorWriter.WriteLine(error.Message);
        }

        return false;
    }
}
}
