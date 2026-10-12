using FraudDetection.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetection.Api.Controllers;

[ApiController]
[Route("api/test/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionClient _transactionClient;

    public TransactionsController(
        ITransactionClient transactionClient)
    {
        _transactionClient = transactionClient;
    }

    [HttpGet("{transactionId}")]
    public async Task<IActionResult> GetTransaction(
        string transactionId,
        CancellationToken cancellationToken)
    {
        var transaction = await _transactionClient.GetByIdAsync(
            transactionId,
            cancellationToken);

        if (transaction == null)
            return NotFound();

        return Ok(transaction);
    }
        [HttpGet("assessments")]
    public async Task<IActionResult> GetAssessments(
        CancellationToken cancellationToken)
    {
        var assessments = await _transactionClient.GetAllAsync(
            cancellationToken);

        return Ok(assessments);
    }
}