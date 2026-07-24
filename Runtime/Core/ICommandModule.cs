using System.Collections.Generic;

namespace TrickleCharge.DingOS
{
public interface ICommandModule<out TCommand>
{
    IEnumerable<TCommand> GetCommands(IShellEnvironment environment);
}
}
