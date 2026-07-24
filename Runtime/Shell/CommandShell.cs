using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.DingOS.Shell
{
public sealed class CommandShell : Command, IShell, IShellEnvironment
{
    private readonly AsyncLocal<TextWriter?> _currentOut = new();
    private readonly AsyncLocal<TextWriter?> _currentErr = new();

    /// <inheritdoc />
    public TextWriter Out => _currentOut.Value ?? TextWriter.Null;

    /// <inheritdoc />
    public TextWriter Error => _currentErr.Value ?? TextWriter.Null;

    /// <inheritdoc />
    public event Action? ClearRequested;

    /// <inheritdoc />
    public event Action? QuitRequested;

    public DateTime StartTime { get; } = DateTime.UtcNow;

    public CommandShell(string name = SystemInfo.ApplicationName, string description = SystemInfo.VersionString)
        : base(name, description)
    {
        Options.Add(new HelpOption());
    }

    public void RegisterModule(ICommandModule<Command> module)
    {
        if (module == null) { throw new ArgumentNullException(nameof(module)); }

        foreach (Command command in module.GetCommands(this))
        {
            RegisterCommand(command);
        }
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
            ParseResult parseResult = Parse(commandLine);

            if(parseResult.Errors.Count == 0)
            {
                InvocationConfiguration config = new()
                {
                    Output = outputScope.OutputWriter,
                    Error = outputScope.ErrorWriter
                };

                exitCode = await parseResult.InvokeAsync(config, cancellationToken);
            }
            else
            {
                PrintParseErrors(parseResult, outputScope.ErrorWriter);
                exitCode = 1;
            }
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

    private static void PrintParseErrors(ParseResult parseResult, TextWriter errorWriter)
    {
        foreach (ParseError error in parseResult.Errors)
        {
            errorWriter.WriteLine(error.Message);
        }
    }
}
}
