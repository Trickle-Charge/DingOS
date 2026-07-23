using TrickleCharge.DingOS.Networking;
using TrickleCharge.DingOS.Shell;

namespace TrickleCharge.DingOS.Devices
{
public interface IDevice
{
    string Name { get; }
    IDeviceDirectory NetworkDirectory { get; }
    ShellContext RequestShell();
}
}
