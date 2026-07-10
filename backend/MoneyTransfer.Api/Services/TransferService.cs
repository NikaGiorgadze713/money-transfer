using Microsoft.EntityFrameworkCore;
using MoneyTransfer.Api.Data;
using MoneyTransfer.Api.Models;

namespace MoneyTransfer.Api.Services;

public class TransferService
{
    private readonly AppDbContext _db;

    public TransferService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TransferResult> ExecuteTransferAsync(int fromAccountId, int toAccountId, decimal amount, CancellationToken ct = default)
    {
        if (amount <= 0)
            return TransferResult.Fail("Amount must be greater than zero.");

        if (fromAccountId == toAccountId)
            return TransferResult.Fail("Cannot transfer to the same account.");

        await using var transaction = await _db.Database.BeginTransactionAsync(ct);

        // Lock both account rows in ascending Id order (not from/to order) so that
        // two concurrent transfers between the same pair of accounts always acquire
        // locks in the same sequence, preventing deadlocks.
        var firstId = Math.Min(fromAccountId, toAccountId);
        var secondId = Math.Max(fromAccountId, toAccountId);

        var firstAccount = await LockAccountAsync(firstId, ct);
        var secondAccount = await LockAccountAsync(secondId, ct);

        var fromAccount = fromAccountId == firstId ? firstAccount : secondAccount;
        var toAccount = toAccountId == firstId ? firstAccount : secondAccount;

        if (fromAccount is null || toAccount is null)
            return TransferResult.Fail("One or both accounts do not exist.");

        if (fromAccount.Balance < amount)
            return TransferResult.Fail("Insufficient funds.");

        fromAccount.Balance -= amount;
        toAccount.Balance += amount;

        _db.TransferTransactions.Add(new TransferTransaction
        {
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            Amount = amount,
            CreatedAtUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return TransferResult.Ok("Transfer completed successfully.");
    }

    private async Task<Account?> LockAccountAsync(int id, CancellationToken ct)
    {
        return await _db.Accounts
            .FromSqlInterpolated($"SELECT * FROM Accounts WITH (UPDLOCK, ROWLOCK) WHERE Id = {id}")
            .SingleOrDefaultAsync(ct);
    }
}
