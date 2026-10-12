using FraudDetection.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FraudDetection.Tests.Middleware;

public class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_WhenTransactionNotFound_ShouldReturn404()
    {
        // Arrange
        var handler = new GlobalExceptionHandler(
            NullLogger<GlobalExceptionHandler>.Instance);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new KeyNotFoundException(
            "Transaction was not found.");

        // Act
        var handled = await handler.TryHandleAsync(
            context,
            exception,
            CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(
            StatusCodes.Status404NotFound,
            context.Response.StatusCode);

        Assert.StartsWith(
            "application/json",
            context.Response.ContentType!);

        context.Response.Body.Position = 0;

        using var document = await System.Text.Json.JsonDocument
            .ParseAsync(context.Response.Body);

        var root = document.RootElement;

        Assert.Equal(
            404,
            root.GetProperty("status").GetInt32());

        Assert.Equal(
            "Not Found",
            root.GetProperty("title").GetString());
    }
}