using System.Collections.Generic;

using TrickleCharge.Sys.DingOS.Devices;

namespace TrickleCharge.Sys.DingOS.Networking
{
public interface IDeviceDirectory : IDictionary<string, IDevice> { }
}
