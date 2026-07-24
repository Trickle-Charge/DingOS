using System.Collections.Generic;

namespace TrickleCharge.DingOS.Core
{
public interface ICommandModule<out TCommand>
{
    IEnumerable<TCommand> GetCommands(IShellEnvironment environment);
}
}
