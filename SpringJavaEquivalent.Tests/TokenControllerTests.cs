// <copyright file="TokenControllerTests.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SpringJavaEquivalent.Controllers;
using SpringJavaEquivalent.Services;
using Xunit;

namespace SpringJavaEquivalent.Tests;

public class TokenControllerTests
{
    private readonly JwtService jwtService;
    private readonly TokenController controller;

    public TokenControllerTests()
    {
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x["Jwt:Secret"]).Returns("test-secret-key-that-is-long-enough-for-hmac-sha256");
        mockConfig.Setup(x => x["Jwt:ExpirationHours"]).Returns("24");
        
        this.jwtService = new JwtService(mockConfig.Object);
        this.controller = new TokenController(this.jwtService);
    }

    [Fact]
    public void GenerateToken_WithValidInputs_ShouldReturnOkResult()
    {
        // Arrange
        var username = "testuser";
        var roles = "USER,ADMIN";
        var permissions = "READ,WRITE";

        // Act
        var result = this.controller.GenerateToken(username, roles, permissions);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult!.Value);
    }

    [Fact]
    public void GeneratePredefinedToken_WithAdminType_ShouldReturnOkResult()
    {
        // Arrange
        var type = "admin";

        // Act
        var result = this.controller.GeneratePredefinedToken(type);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GenerateAdminToken_ShouldReturnOkResult()
    {
        // Act
        var result = this.controller.GenerateAdminToken();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GenerateUserToken_ShouldReturnOkResult()
    {
        // Act
        var result = this.controller.GenerateUserToken();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnOkResult()
    {
        // Arrange
        var request = new Dictionary<string, string> { { "token", "valid-token" } };

        // Act
        var result = this.controller.ValidateToken(request);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
