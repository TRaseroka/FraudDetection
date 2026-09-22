using System.Net;
using System.Net.Http.Json;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;

namespace FraudDetection.Persistence.Clients;

public class TransactionClient : ITransactionClient
{
    private readonly HttpClient _httpClient;

    public TransactionClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TransactionDto?> GetByIdAsync(
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            $"/api/transactions/{transactionId}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TransactionDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyCollection<TransactionDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var transactions =
            await _httpClient.GetFromJsonAsync<List<TransactionDto>>(
                "/api/transactions",
                cancellationToken);

        return transactions ?? [];
    }
}