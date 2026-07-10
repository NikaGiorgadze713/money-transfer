namespace MoneyTransfer.Api.Services;

public class TransferResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;

    public static TransferResult Ok(string message) => new() { Success = true, Message = message };
    public static TransferResult Fail(string message) => new() { Success = false, Message = message };
}
