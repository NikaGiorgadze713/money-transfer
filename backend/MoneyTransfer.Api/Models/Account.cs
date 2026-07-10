namespace MoneyTransfer.Api.Models;

public class Account
{
    public int Id { get; set; }

    public required string Owner { get; set; }

    public decimal Balance { get; set; }
}
