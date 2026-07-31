# DingOS

[![Static Badge](https://img.shields.io/badge/DingOS-brightgreen?logo=github&label=GitHub)](https://github.com/Trickle-Charge/DingOS)


**DingOS** is a decoupled terminal architecture and context stack manager for .NET and Unity applications. 
It decouples user input/output interfaces from command execution logic, allowing you to host interactive shell 
environments in CLI applications, Unity UI frameworks, or remote debugging tools.

Out of the box, DingOS includes a full execution engine implementation powered by `System.CommandLine`.

---

## Core Concepts

DingOS separates terminal operations into four primary components:

| Component                | Responsibility                                                             |
|:-------------------------|:---------------------------------------------------------------------------|
| **`ITerminal`**          | Handles low-level string I/O (e.g., OS Console, Unity UI InputField/Text). |
| **`IShell`**             | Parses raw command strings and executes logic.                             |
| **`IShellContextStack`** | Manages modal session contexts and prompt state (`ShellContextManager`).   |
| **`ITerminalHost`**      | Bridges the Terminal and Context Stack together.                           |

---

## Quickstart

### Standalone Console Execution (Pull Model)

For CLI tools or standalone console applications running a blocking execution loop:

```csharp
using TrickleCharge.DingOS.Core;
using TrickleCharge.DingOS.Shell;
using TrickleCharge.DingOS.Terminal;

// 1. Initialize Terminal & Shell Engine
var terminal = new ConsoleTerminal();
var shell = new CommandShell().WithInteractiveDefaults();

// 2. Setup Context Manager & Push Initial Context
var contextManager = new ShellContextManager(terminal);
contextManager.PushContext(new ShellContext("Root", "> ", shell));

// 3. Host and Run Loop
using var host = new TerminalHost(terminal, contextManager);
await host.RunConsoleLoopAsync();
```

### Unity & UI Execution (Push Model)

Unlike CLI console applications that run a continuous blocking read loop (`RunConsoleLoopAsync`), event-driven 
environments like Unity UI (UI Toolkit, TextMeshPro, Canvas UGUI) or Web frontends operate using a **Push Model**. 

Instead of locking a thread to wait for standard input, the UI framework captures user input events 
(such as pressing `Enter` in an input field) and pushes command strings into the shell via `ITerminalHost.ExecuteAsync()`.

#### Quickstart Example

Operating the engine in a push environment boils down to two steps: initializing the pipeline and 
invoking `ExecuteAsync` on user submission.

Here is a minimal controller demonstrating how to drive `DingOS` using the push execution model:

```csharp
// 1. Pipeline Setup
UnityTerminal terminal = new UnityTerminal(view);
CommandShell shell = new CommandShell().WithInteractiveDefaults();

ShellContextManager contextStack = new ShellContextManager(terminal);
contextStack.PushContext(new ShellContext("Runtime", "> ", shell));

TerminalHost host = new TerminalHost(terminal, contextStack);

// 2. Execution Push (e.g., called from your UI's submit event)
async Task SubmitCommandAsync(string rawInput, CancellationToken ct)
{
    // Echo the active prompt + input to the view
    terminal.WriteLine($"{host.ContextStack.ActivePrompt}{rawInput}");

    // Push the command line into the engine
    await host.ExecuteAsync(rawInput, ct);
}
```

**Key Components**

* **`UnityTerminal`**: Implementation of `ITerminal` that redirects standard output and standard error streams to a
  display. (e.g. Custom `EditorWindow` or in game UI elements.)
* **`ITerminalHost.ExecuteAsync(string, CancellationToken)`**: The core push API endpoint that executes a single input 
string against the currently active shell context.

#### Important Considerations

* **Prompt Echoing**: Because input submission is event-driven, your UI layer should explicitly print the active prompt 
(`host.ContextStack.ActivePrompt`) and user input line prior to invoking `ExecuteAsync()`.
* **Dynamic Prompts**: If an executed command modifies context state (e.g., connecting to a remote device or switching 
sub-shells), `ActivePrompt` automatically updates to match the new top context.
* **Cancellation Support**: Always pass a `CancellationToken` to `ExecuteAsync` so long-running operations or multi-tick 
commands can interrupt cleanly when the UI closes or disables.

---

## Creating Command Modules

Commands are organized into modules implementing `ICommandModule<Command>`.

```csharp
using System.Collections.Generic;
using System.CommandLine;
using TrickleCharge.DingOS.Core;
using UnityEngine;

public class TimeScaleModule : ICommandModule<Command>
{
    public IEnumerable<Command> GetCommands(IShellEnvironment environment)
    {
        // 'set' and 'reset' commands nested under 'timescale' command
        // > timescale set 0.5
        // > timescale reset
        Command cmd = new("timescale", "Modify Time.timescale")
        {
            TimeScaleSet(environment),
            TimeScaleReset(environment)
        };

        yield return cmd;
    }

    private static Command TimeScaleSet(IShellEnvironment env)
    {
        Argument<float> scaleArg = new("scale") { Description = "Target simulation timescale." };
        Command cmd = new("set", "Set the game simulation speed.") { scaleArg };

        cmd.SetAction(parseResult =>
        {
            float scale = parseResult.GetValue(scaleArg);

            Time.timeScale = scale;

            // Route output safely through the environment writer
            env.Out.WriteLine($"Simulation timescale set to {scale}");
        });

        return cmd;
    }

    private static Command TimeScaleReset(IShellEnvironment env)
    {
        Command cmd = new("reset", "Set the game simulation speed to 1.");

        cmd.SetAction(_ =>
        {
            Time.timeScale = 1;

            env.Out.WriteLine("Simulation timescale reset to 1");
        });

        return cmd;
    }
}
```

> See: `TrickleCharge.DingOS.Unity.Modules.TimeScaleModule`

Register modules with your shell instance:

```csharp
shell.RegisterModule(new TimeScaleModule());
```

### `ICommandModule<T>.GetCommands` usage

`GetCommands` returns an `IEnumerable` of the given command type.

This means a module can return multiple commands by yielding each in sequence.

```csharp
public class CoreShellModule : ICommandModule<Command>
{
    // ...
    
    public IEnumerable<Command> GetCommands(IShellEnvironment environment)
    {
        yield return Exit(environment);
        yield return Clear(environment);
        yield return Help(environment, _shell);
    }
    
    // ...
}
```

> See: `TrickleCharge.DingOS.Modules.CoreShellModule`

---