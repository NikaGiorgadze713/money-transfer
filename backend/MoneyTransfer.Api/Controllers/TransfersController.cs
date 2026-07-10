using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.Api.Dtos;
using MoneyTransfer.Api.Services;

namespace MoneyTransfer.Api.Controllers;

[ApiController]
[Route("api/transfers")]
public class TransfersController : ControllerBase
{
    private readonly TransferService _transferService;

    public TransfersController(TransferService transferService)
    {
        _transferService = transferService;
    }

    [HttpPost]
    public async Task<ActionResult<TransferResponseDto>> CreateTransfer(TransferRequestDto request, CancellationToken ct)
    {
        var result = await _transferService.ExecuteTransferAsync(request.FromAccountId, request.ToAccountId, request.Amount, ct);

        if (!result.Success)
            return BadRequest(new TransferResponseDto(result.Message));

        return Ok(new TransferResponseDto(result.Message));
    }
}
