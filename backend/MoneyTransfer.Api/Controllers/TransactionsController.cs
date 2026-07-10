using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTransfer.Api.Data;
using MoneyTransfer.Api.Dtos;

namespace MoneyTransfer.Api.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TransactionsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions(CancellationToken ct)
    {
        var transactions = await _db.TransferTransactions
            .OrderByDescending(t => t.CreatedAtUtc)
            .Select(t => new TransactionDto(
                t.Id,
                t.FromAccountId,
                t.FromAccount.Owner,
                t.ToAccountId,
                t.ToAccount.Owner,
                t.Amount,
                t.CreatedAtUtc))
            .ToListAsync(ct);

        return Ok(transactions);
    }
}
