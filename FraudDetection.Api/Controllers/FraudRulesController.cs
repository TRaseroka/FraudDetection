using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetection.Api.Controllers;

[ApiController]
[Route("api/fraud-rules")]
public class FraudRulesController : ControllerBase
{
    private readonly IFraudRuleManagementService _ruleManagementService;

    public FraudRulesController(
        IFraudRuleManagementService ruleManagementService)
    {
        _ruleManagementService = ruleManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<FraudRuleConfiguration>>> GetAll(
        CancellationToken cancellationToken)
    {
        var rules = await _ruleManagementService.GetAllAsync(
            cancellationToken);

        return Ok(rules);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<FraudRuleConfiguration>> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var rule = await _ruleManagementService.GetByCodeAsync(
            code,
            cancellationToken);

        return Ok(rule);
    }

    [HttpPatch("{code}/enable")]
    public async Task<IActionResult> Enable(
        string code,
        CancellationToken cancellationToken)
    {
        await _ruleManagementService.SetEnabledAsync(
            code,
            true,
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{code}/disable")]
    public async Task<IActionResult> Disable(
        string code,
        CancellationToken cancellationToken)
    {
        await _ruleManagementService.SetEnabledAsync(
            code,
            false,
            cancellationToken);

        return NoContent();
    }
}