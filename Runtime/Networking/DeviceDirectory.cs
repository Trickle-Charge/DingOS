using System.Collections.Generic;

using TrickleCharge.Sys.DingOS.Devices;

namespace TrickleCharge.Sys.DingOS.Networking
{
public class DeviceDirectory : Dictionary<string, IDevice>, IDeviceDirectory { }
}
