using System.Threading;
using System.Threading.Tasks;

namespace TrickleCharge.Sys.DingOS
{
public interface ITerminal
{
    void Write(string text);
    void WriteLine(string text);
    void WriteError(string text);
    string ReadLine();
    Task<string> ReadLineAsync(CancellationToken cancellationToken = default);
    void Clear();
}
}
