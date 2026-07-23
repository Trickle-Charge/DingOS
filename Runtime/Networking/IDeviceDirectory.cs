using System.Collections.Generic;

using TrickleCharge.DingOS.Devices;

namespace TrickleCharge.DingOS.Networking
{
public interface IDeviceDirectory : IDictionary<string, IDevice> { }
}
