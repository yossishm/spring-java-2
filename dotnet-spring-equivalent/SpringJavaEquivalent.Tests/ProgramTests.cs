// <copyright file="ProgramTests.cs" company="SpringJavaEquivalent">
// Copyright (c) SpringJavaEquivalent. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Tests
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using SpringJavaEquivalent.Services;
    using Xunit;

    /// <summary>
    /// Integration tests for the Program class.
    /// </summary>
    public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramTests"/> class.
        /// </summary>
        /// <param name="factory">The web application factory.</param>
        public ProgramTests(WebApplicationFactory<Program> factory)
        {
            this.this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Environment", "Testing");
            });
        }

        /// <summary>
        /// Verifies that the application can be created by the factory without errors.
        /// </summary>
        [Fact]
        public void Application_StartsSuccessfully()
        {
            // Arrange & Act
            var client = this.this.factory.CreateClient();

            // Assert
            Assert.NotNull(client);
        }

        /// <summary>
        /// Verifies that a request to a public endpoint succeeds, testing the request pipeline.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task PublicEndpoint_ReturnsOk()
        {
            // Arrange
            var client = this.this.factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/enhanced-test/public");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Verifies that essential services are registered in the DI container.
        /// </summary>
        /// <param name="serviceType">The service type to check.</param>
        [Theory]
        [InlineData(typeof(JwtService))]
        [InlineData(typeof(IAuthorizationHandler))]
        public void Services_AreRegistered(Type serviceType)
        {
            // Arrange
            var services = this.this.factory.Services;

            // Act
            var service = services.GetService(serviceType);

            // Assert
            Assert.NotNull(service);
        }

        /// <summary>
        /// Verifies that all custom authorization policies are registered.
        /// </summary>
        /// <param name="policyName">The policy to check.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("CacheReadPolicy")]
        [InlineData("CacheWritePolicy")]
        [InlineData("AdminOrPowerUserPolicy")]
        [InlineData("AdminAndCacheWritePolicy")]
        [InlineData("IdentityProviderPolicy")]
        [InlineData("AuthLevelPolicy")]
        public async Task AuthorizationPolicies_AreRegistered(string policyName)
        {
            // Arrange
            var services = this.this.factory.Services;
            var policyProvider = services.GetRequiredService<IAuthorizationPolicyProvider>();

            // Act
            var policy = await policyProvider.GetPolicyAsync(policyName);

            // Assert
            Assert.NotNull(policy);
        }
    }
}