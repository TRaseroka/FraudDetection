using FraudDetection.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetection.Api.Controllers;

[ApiController]
[Route("api/fraud")]
public class FraudController : ControllerBase
{
    private readonly IFraudAssessmentService _fraudService;
    private readonly IFraudAssessmentRepository _repository;

    public FraudController(
        IFraudAssessmentService fraudService,
        IFraudAssessmentRepository repository)
    {
        _fraudService = fraudService;
        _repository = repository;
    }

    [HttpPost("assess/{transactionId}")]
    public async Task<IActionResult> Assess(
        string transactionId,
        CancellationToken cancellationToken)
    {
        var result = await _fraudService.AssessTransactionAsync(
            transactionId,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("assessments")]
    public async Task<IActionResult> GetAssessments(
        CancellationToken cancellationToken)
    {
        var assessments = await _repository.GetAllAsync(
            cancellationToken);

        return Ok(assessments);
    }

    [HttpGet("assessments/{transactionId}")]
    public async Task<IActionResult> GetAssessment(
        string transactionId,
        CancellationToken cancellationToken)
    {
        var assessment = await _repository.GetByTransactionIdAsync(
            transactionId,
            cancellationToken);

        if (assessment == null)
            return NotFound();

        return Ok(assessment);
    }
}