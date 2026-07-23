using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.Sys.DingOS.Terminal
{
    public sealed class TerminalHost : IDisposable
    {
        private readonly ITerminal _terminal;
        private readonly TerminalTextWriter _stdOut;
        private readonly TerminalTextWriter _stdErr;

        public IShellContextStack ContextStack { get; }

        public TerminalHost(ITerminal terminal, IShellContextStack contextStack)
        {
            _terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            ContextStack = contextStack ?? throw new ArgumentNullException(nameof(contextStack));

            // Create persistent stream wrappers for this terminal session
            _stdOut = new TerminalTextWriter(_terminal.Write, _terminal.WriteLine);
            _stdErr = new TerminalTextWriter(_terminal.WriteError, _terminal.WriteErrorLine);
        }

        /// <summary>
        /// Runs a continuous blocking ReadLine loop for OS console environments. (Pull API - CLI Playground)
        /// </summary>
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

        /// <summary>
        /// Executes a single input command against the current active context. (Push API - Unity / UI / Web)
        /// </summary>
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
            _stdOut.Dispose();
            _stdErr.Dispose();
        }
    }
}