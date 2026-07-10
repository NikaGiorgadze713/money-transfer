using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTransfer.Api.Data;
using MoneyTransfer.Api.Dtos;

namespace MoneyTransfer.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AccountsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccounts(CancellationToken ct)
    {
        var accounts = await _db.Accounts
            .OrderBy(a => a.Id)
            .Select(a => new AccountDto(a.Id, a.Owner, a.Balance))
            .ToListAsync(ct);

        return Ok(accounts);
    }
}
