using FraudDetection.Persistence;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FraudDetection.Application.Interfaces;
using FraudDetection.Persistence.Clients;
using FraudDetection.Application.Services;
using FraudDetection.Application.Rules;
using FraudDetection.Persistence.Repositories;
using FraudDetection.Application.Configuration;
using FraudDetection.Api.Middleware;
using FraudDetection.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<FraudDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("FraudDb")));

// Repositories
builder.Services.AddScoped<IFraudAssessmentRepository,FraudAssessmentRepository>();
builder.Services.AddScoped<IFraudRuleConfigurationRepository,FraudRuleConfigurationRepository>();

// Fraud Rules
builder.Services.AddScoped<IFraudRule, HighValueTransactionRule>();
builder.Services.AddScoped<IFraudRule, CashDepositRule>();
builder.Services.AddScoped<IFraudRule, TransactionVelocityRule>();

// Services
builder.Services.AddScoped< IFraudAssessmentService, FraudAssessmentService>();
builder.Services.AddScoped< IFraudRuleManagementService,FraudRuleManagementService>();
// Fraud Rule Configuration
builder.Services.Configure<FraudRuleOptions>(
    builder.Configuration.GetSection(
        FraudRuleOptions.SectionName));
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(FraudMappingProfile).Assembly);

// Transaction API Client
builder.Services.AddHttpClient<
    ITransactionClient,
    TransactionClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["TransactionApi:BaseUrl"]!);
});

// Global Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Exception Handling
app.UseExceptionHandler();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Controllers
app.MapControllers();

app.Run();