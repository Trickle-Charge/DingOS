using System.Collections.Generic;

using TrickleCharge.DingOS.Devices;

namespace TrickleCharge.DingOS.Networking
{
public class DeviceDirectory : Dictionary<string, IDevice>, IDeviceDirectory { }
}
