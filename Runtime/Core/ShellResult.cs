namespace TrickleCharge.DingOS.Core
{
public readonly struct ShellResult
{
    public int ExitCode { get; }
    public string Output { get; }
    public string Error { get; }

    public bool IsSuccess => ExitCode == 0;

    public ShellResult(int exitCode, string output, string error)
    {
        ExitCode = exitCode;
        Output = string.IsNullOrWhiteSpace(output) ? string.Empty : output;
        Error = string.IsNullOrWhiteSpace(error) ? string.Empty : error;
    }

    public static ShellResult Empty => new(0, string.Empty, string.Empty);
}
}
