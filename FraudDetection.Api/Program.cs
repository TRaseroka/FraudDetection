using FraudDetection.Persistence;
using Microsoft.EntityFrameworkCore;
using FraudDetection.Application.Interfaces;
using FraudDetection.Persistence.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<FraudDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("FraudDb")));
// Add Entity Framework services


builder.Services.AddHttpClient<ITransactionClient, TransactionClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["TransactionApi:BaseUrl"]!);
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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


app.Run();


