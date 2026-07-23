namespace TrickleCharge.Sys.DingOS
{
public interface ITerminal
{
    void Write(string text);
    void WriteLine(string text);

    void WriteError(string text);
    void WriteErrorLine(string text);

    string ReadLine();
    void Clear();
}
}
