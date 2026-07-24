namespace TrickleCharge.DingOS
{
public interface ICommandModule<in TShell> where TShell : IShell
{
    void Register(TShell shell);
}
}
