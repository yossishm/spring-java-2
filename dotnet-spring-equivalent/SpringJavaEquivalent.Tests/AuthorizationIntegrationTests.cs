// <copyright file="AuthorizationIntegrationTests.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using SpringJavaEquivalent.Services;
    using Xunit;

    public class AuthorizationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly HttpClient client;

        public AuthorizationIntegrationTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Environment", "Testing");
            });
            this.client = this.factory.CreateClient();
        }

        [Fact]
        public async Task PublicEndpoint_ShouldReturnOk_WithoutAuthentication()
        {
            // Act
            var response = await this.client.GetAsync("/api/v1/enhanced-test/public");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AuthenticatedEndpoint_ShouldReturnUnauthorized_WithoutToken()
        {
            // Act
            var response = await this.client.GetAsync("/api/v1/enhanced-test/authenticated");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AuthenticatedEndpoint_ShouldReturnOk_WithValidToken()
        {
            // Arrange
            var jwtService = this.factory.Services.GetRequiredService<JwtService>();
            var token = jwtService.GenerateToken("testuser", new[] { "USER" }, new[] { "CACHE_READ" });

            this.client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await this.client.GetAsync("/api/v1/enhanced-test/authenticated");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AdminEndpoint_ShouldReturnForbidden_WithUserRole()
        {
            // Arrange
            var jwtService = this.factory.Services.GetRequiredService<JwtService>();
            var token = jwtService.GenerateToken("testuser", new[] { "USER" }, new[] { "CACHE_READ" });

            this.client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await this.client.GetAsync("/api/v1/enhanced-test/admin-role");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AdminEndpoint_ShouldReturnOk_WithAdminRole()
        {
            // Arrange
            var jwtService = this.factory.Services.GetRequiredService<JwtService>();
            var token = jwtService.GenerateToken("admin", new[] { "ADMIN" }, new[] { "CACHE_READ", "CACHE_WRITE", "ADMIN_ACCESS" });

            this.client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await this.client.GetAsync("/api/v1/enhanced-test/admin-role");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CacheReadEndpoint_ShouldReturnOk_WithReadPermission()
        {
            // Arrange
            var jwtService = this.factory.Services.GetRequiredService<JwtService>();
            var token = jwtService.GenerateToken("testuser", new[] { "USER" }, new[] { "CACHE_READ" });

            this.client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await this.client.GetAsync("/api/v1/enhanced-test/cache-read");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CacheWriteEndpoint_ShouldReturnForbidden_WithoutWritePermission()
        {
            // Arrange
            var jwtService = this.factory.Services.GetRequiredService<JwtService>();
            var token = jwtService.GenerateToken("testuser", new[] { "USER" }, new[] { "CACHE_READ" });

            this.client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await this.client.GetAsync("/api/v1/enhanced-test/cache-write");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CacheWriteEndpoint_ShouldReturnOk_WithWritePermission()
        {
            // Arrange
            var jwtService = this.factory.Services.GetRequiredService<JwtService>();
            var token = jwtService.GenerateToken("testuser", new[] { "USER" }, new[] { "CACHE_WRITE" });

            this.client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await this.client.GetAsync("/api/v1/enhanced-test/cache-write");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AuthLevelEndpoint_ShouldReturnOk_WithAAL2()
        {
            // Arrange
            var jwtService = this.factory.Services.GetRequiredService<JwtService>();
            var token = jwtService.GenerateToken("testuser", new[] { "USER" }, new[] { "CACHE_READ" }, "AAL2");

            this.client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await this.client.GetAsync("/api/v1/enhanced-test/auth-level");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task IdentityProviderEndpoint_ShouldReturnOk_WithEnterpriseProvider()
        {
            // Arrange
            var jwtService = this.factory.Services.GetRequiredService<JwtService>();
            var token = jwtService.GenerateToken("testuser", new[] { "USER" }, new[] { "CACHE_READ" }, "AAL1", "enterprise");

            this.client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await this.client.GetAsync("/api/v1/enhanced-test/identity-provider");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task MaximumSecurityEndpoint_ShouldReturnOk_WithAllRequirements()
        {
            // Arrange
            var jwtService = this.factory.Services.GetRequiredService<JwtService>();
            var token = jwtService.GenerateToken("admin", new[] { "ADMIN" }, new[] { "CACHE_READ", "CACHE_WRITE", "ADMIN_ACCESS" }, "AAL2", "enterprise");

            this.client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await this.client.GetAsync("/api/v1/enhanced-test/maximum-security");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}