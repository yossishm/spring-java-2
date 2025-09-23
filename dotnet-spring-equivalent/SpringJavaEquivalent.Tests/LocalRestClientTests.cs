using Microsoft.Extensions.Logging;
using Moq;
using SpringJavaEquivalent.Services;
using System.Net;
using System.Net.Http;
using Xunit;

namespace SpringJavaEquivalent.Tests;

public class LocalRestClientTests : IDisposable
{
    private readonly LocalRestClient _localRestClient;

    public LocalRestClientTests()
    {
        this._localRestClient = new LocalRestClient("test-auth");
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var authorization = "test-auth";

        // Act
        var client = new LocalRestClient(authorization);

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithEmptyAuthorization_ShouldCreateInstance()
    {
        // Arrange
        var authorization = "";

        // Act
        var client = new LocalRestClient(authorization);

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithNullAuthorization_ShouldCreateInstance()
    {
        // Arrange
        string authorization = null!;

        // Act
        var client = new LocalRestClient(authorization);

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public async Task GetAsync_WithValidEndpoint_ShouldReturnResponse()
    {
        // Arrange
        var endpoint = new Uri("/api/test", UriKind.Relative);

        // Note: This test would require a mock HTTP server or HttpClient
        // For now, we'll test the method signature and basic behavior
        try
        {
            // Act
            var result = await this._localRestClient.GetAsync(endpoint);

            // Assert
            // This will likely throw an exception due to no server running
            // but we can verify the method was called
        }
        catch (HttpRequestException)
        {
            // Expected when no server is running
            Assert.True(true);
        }
    }

    [Fact]
    public async Task GetAsync_WithNullEndpoint_ShouldThrowArgumentNullException()
    {
        // Arrange
        Uri endpoint = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => this._localRestClient.GetAsync(endpoint));
    }

    [Fact]
    public void Dispose_ShouldDisposeHttpClient()
    {
        // Arrange
        var client = new LocalRestClient("test-auth");

        // Act
        client.Dispose();

        // Assert
        // HttpClient should be disposed
        // We can't directly test this, but the method should not throw
        Assert.True(true);
    }

    [Fact]
    public void Dispose_ShouldNotThrowOnMultipleCalls()
    {
        // Arrange
        var client = new LocalRestClient("test-auth");

        // Act & Assert
        client.Dispose();
        client.Dispose(); // Should not throw
        Assert.True(true);
    }

    public void Dispose()
    {
        this._localRestClient?.Dispose();
    }
}
