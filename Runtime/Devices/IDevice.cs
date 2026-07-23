using TrickleCharge.Sys.DingOS.Networking;
using TrickleCharge.Sys.DingOS.Shell;

namespace TrickleCharge.Sys.DingOS.Devices
{
public interface IDevice
{
    string Name { get; }
    IDeviceDirectory NetworkDirectory { get; }
    ShellContext RequestShell();
}
}
