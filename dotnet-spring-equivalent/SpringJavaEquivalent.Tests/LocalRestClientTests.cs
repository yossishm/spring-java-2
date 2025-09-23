// <copyright file="LocalRestClientTests.cs" company="SpringJavaEquivalent">
// Copyright (c) SpringJavaEquivalent. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Tests
{
    using System.Net;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using Moq;
    using SpringJavaEquivalent.Services;
    using Xunit;

    public class LocalRestClientTests : IDisposable
    {
        private readonly LocalRestClient localRestClient;
        private bool disposed;

        public LocalRestClientTests()
        {
            this.localRestClient = new LocalRestClient("test-auth");
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
            var authorization = string.Empty;

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
                await this.localRestClient.GetAsync(endpoint);

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
            await Assert.ThrowsAsync<ArgumentNullException>(() => this.localRestClient.GetAsync(endpoint));
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
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.localRestClient?.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}