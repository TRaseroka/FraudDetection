using FraudDetection.Persistence;
using Microsoft.EntityFrameworkCore;
using FraudDetection.Application.Interfaces;
using FraudDetection.Persistence.Clients;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Services;
using FraudDetection.Application.Rules;
using FraudDetection.Persistence.Repositories;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FraudDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("FraudDb")));
// Add Entity Framework services
builder.Services.AddScoped< IFraudAssessmentRepository, FraudAssessmentRepository>();
builder.Services.AddScoped<IFraudRule, HighValueTransactionRule>();
builder.Services.AddScoped<IFraudRule, CashDepositRule>();
builder.Services.AddScoped<IFraudRule, TransactionVelocityRule>();
builder.Services.AddScoped<IFraudAssessmentService, FraudAssessmentService>();
builder.Services.AddHttpClient<ITransactionClient, TransactionClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["TransactionApi:BaseUrl"]!);
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/test/transactions",
    async (
        ITransactionClient transactionClient,
        CancellationToken cancellationToken) =>
    {
        var transactions =
            await transactionClient.GetAllAsync(cancellationToken);

        return Results.Ok(transactions);
    });
app.MapGet("/api/test/transactions/{transactionId}",
    async (
        string transactionId,
        ITransactionClient transactionClient,
        CancellationToken cancellationToken) =>
    {
        var transaction = await transactionClient.GetByIdAsync(
            transactionId,
            cancellationToken);

        return transaction is null
            ? Results.NotFound()
            : Results.Ok(transaction);
    })
    .WithName("GetTransactionById");

app.MapPost("/api/fraud/assess/{transactionId}",
    async (
        string transactionId,
        IFraudAssessmentService fraudAssessmentService,
        CancellationToken cancellationToken) =>
    {
        var result = await fraudAssessmentService.AssessTransactionAsync(
            transactionId,
            cancellationToken);

        return result is null
            ? Results.NotFound()
            : Results.Ok(result);
    });

app.MapGet("/api/fraud/assessments",
    async (
        IFraudAssessmentRepository repository,
        CancellationToken cancellationToken) =>
    {
        var assessments =
            await repository.GetAllAsync(cancellationToken);

        return Results.Ok(assessments);
    })
    .WithName("GetFraudAssessments");

app.MapGet("/api/fraud/assessments/{transactionId}",
    async (
        string transactionId,
        IFraudAssessmentRepository repository,
        CancellationToken cancellationToken) =>
    {
        var assessments =
            await repository.GetByTransactionIdAsync(
                transactionId,
                cancellationToken);

        return assessments.Count == 0
            ? Results.NotFound()
            : Results.Ok(assessments);
    })
    .WithName("GetFraudAssessmentsByTransactionId");
app.Run();


