namespace MoneyTransfer.Api.Dtos;

public record TransactionDto(
    int Id,
    int FromAccountId,
    string FromOwner,
    int ToAccountId,
    string ToOwner,
    decimal Amount,
    DateTime CreatedAtUtc);
