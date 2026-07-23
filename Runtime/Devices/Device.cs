using TrickleCharge.DingOS.Networking;
using TrickleCharge.DingOS.Shell;

namespace TrickleCharge.DingOS.Devices
{
public class Device : IDevice
{
    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public IDeviceDirectory NetworkDirectory { get; } = new DeviceDirectory();

    /// <inheritdoc />
    public ShellContext RequestShell()
    {
        CommandShell shell = new CommandShell()
            .WithInteractiveDefaults();

        ShellContext rootContext = new(
            name: Name,
            prompt: $"{Name}> ",
            commandShell: shell
        );

        return rootContext;
    }

    public Device(string name) => Name =  name;
}
}
