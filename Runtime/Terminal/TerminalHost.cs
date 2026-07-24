    using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.DingOS.Terminal
{
    public sealed class TerminalHost : ITerminalHost, IDisposable
    {
        private readonly ITerminal _terminal;
        private readonly TextWriter _stdOut;
        private readonly TextWriter _stdErr;

        public IShellContextStack ContextStack { get; }

        private readonly bool _ownsWriters;

        public TerminalHost(ITerminal terminal, IShellContextStack contextStack)
        : this(
            terminal ?? throw new ArgumentNullException(nameof(terminal)),
            contextStack,
            new TerminalTextWriter(terminal.Write, terminal.WriteLine),
            new TerminalTextWriter(terminal.WriteError, terminal.WriteErrorLine),
            ownsWriters: true) { }

        public TerminalHost(
            ITerminal terminal,
            IShellContextStack contextStack,
            TextWriter stdOut,
            TextWriter stdErr,
            bool ownsWriters = false)
        {
            _terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            ContextStack = contextStack ?? throw new ArgumentNullException(nameof(contextStack));

            _stdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            _stdErr = stdErr ?? throw new ArgumentNullException(nameof(stdErr));

            _ownsWriters = ownsWriters;
        }

        /// <inheritdoc />
        public async Task RunConsoleLoopAsync(CancellationToken cancellationToken = default)
        {
            _terminal.WriteLine($"Welcome to {SystemInfo.VersionString}");
            _terminal.WriteLine("Type 'help' for available commands or 'exit' to quit.\n");

            while (ContextStack.CurrentContext != null && !cancellationToken.IsCancellationRequested)
            {
                _terminal.Write(ContextStack.ActivePrompt);
                string input = _terminal.ReadLine();

                await ExecuteAsync(input, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task<ShellResult> ExecuteAsync(string input, CancellationToken cancellationToken = default)
        {
            if (ContextStack.CurrentContext == null)
            {
                return ShellResult.Empty;
            }

            return await ContextStack.CurrentContext.ProcessInputAsync(
                input,
                _stdOut,
                _stdErr,
                cancellationToken
            );
        }

        public void Dispose()
        {
            if (_ownsWriters)
            {
                _stdOut.Dispose();
                _stdErr.Dispose();
            }
        }
    }
}