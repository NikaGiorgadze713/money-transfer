namespace MoneyTransfer.Api.Dtos;

public record TransferRequestDto(int FromAccountId, int ToAccountId, decimal Amount);
