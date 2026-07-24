namespace TrickleCharge.DingOS.Core
{
public interface IShellContextStack
{
    IShellContext? CurrentContext { get; }
    string ActivePrompt { get; }
    void PushContext(IShellContext context);
    void PopContext();
}
}
