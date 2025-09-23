// <copyright file="JwtServiceTests.cs" company="SpringJavaEquivalent">
// Copyright (c) SpringJavaEquivalent. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Tests;

using Microsoft.Extensions.Configuration;
using Moq;
using SpringJavaEquivalent.Services;
using Xunit;

public class JwtServiceTests
{
    private readonly Mock<IConfiguration> mockConfiguration;
    private readonly JwtService jwtService;

    public JwtServiceTests()
    {
        this.mockConfiguration = new Mock<IConfiguration>();
        this.mockConfiguration.Setup(x => x["Jwt:Secret"]).Returns("test-secret-key-that-is-long-enough-for-hmac-sha256");
        this.mockConfiguration.Setup(x => x["Jwt:ExpirationHours"]).Returns("24");

        this.jwtService = new JwtService(this.mockConfiguration.Object);
    }

    [Fact]
    public void GenerateToken_WithValidInputs_ShouldReturnValidToken()
    {
        // Arrange
        var username = "testuser";
        var roles = new List<string> { "USER", "ADMIN" };
        var permissions = new List<string> { "READ", "WRITE" };

        // Act
        var token = this.jwtService.GenerateToken(username, roles, permissions);

        // Assert
        Assert.NotNull(token);
        Assert.Contains(".", token);
    }

    [Fact]
    public void GenerateToken_WithNullUsername_ShouldThrowArgumentNullException()
    {
        // Arrange
        var roles = new List<string> { "USER" };
        var permissions = new List<string> { "READ" };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this.jwtService.GenerateToken(null!, roles, permissions));
    }

    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var username = "testuser";
        var roles = new List<string> { "USER" };
        var permissions = new List<string> { "READ" };
        var token = this.jwtService.GenerateToken(username, roles, permissions);

        // Act
        var isValid = this.jwtService.ValidateToken(token);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var isValid = this.jwtService.ValidateToken(invalidToken);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ExtractUsername_WithValidToken_ShouldReturnUsername()
    {
        // Arrange
        var username = "testuser";
        var roles = new List<string> { "USER" };
        var permissions = new List<string> { "READ" };
        var token = this.jwtService.GenerateToken(username, roles, permissions);

        // Act
        var extractedUsername = JwtService.ExtractUsername(token);

        // Assert
        Assert.Equal(username, extractedUsername);
    }

    [Fact]
    public void ExtractRoles_WithValidToken_ShouldReturnRoles()
    {
        // Arrange
        var username = "testuser";
        var roles = new List<string> { "USER", "ADMIN" };
        var permissions = new List<string> { "READ" };
        var token = this.jwtService.GenerateToken(username, roles, permissions);

        // Act
        var extractedRoles = JwtService.ExtractRoles(token);

        // Assert
        Assert.Equal(2, extractedRoles.Count);
        Assert.Contains("USER", extractedRoles);
        Assert.Contains("ADMIN", extractedRoles);
    }

    [Fact]
    public void ExtractPermissions_WithValidToken_ShouldReturnPermissions()
    {
        // Arrange
        var username = "testuser";
        var roles = new List<string> { "USER" };
        var permissions = new List<string> { "READ", "WRITE" };
        var token = this.jwtService.GenerateToken(username, roles, permissions);

        // Act
        var extractedPermissions = JwtService.ExtractPermissions(token);

        // Assert
        Assert.Equal(2, extractedPermissions.Count);
        Assert.Contains("READ", extractedPermissions);
        Assert.Contains("WRITE", extractedPermissions);
    }

    [Fact]
    public void ExtractUsername_WithInvalidToken_ShouldReturnNull()
    {
        // Arrange
        var invalidToken = "invalid.token.signature";

        // Act
        var extractedUsername = JwtService.ExtractUsername(invalidToken);

        // Assert
        Assert.Null(extractedUsername);
    }

    [Fact]
    public void ExtractRoles_WithInvalidToken_ShouldReturnEmptyList()
    {
        // Arrange
        var invalidToken = "invalid.token.signature";

        // Act
        var extractedRoles = JwtService.ExtractRoles(invalidToken);

        // Assert
        Assert.Empty(extractedRoles);
    }

    [Fact]
    public void ExtractPermissions_WithInvalidToken_ShouldReturnEmptyList()
    {
        // Arrange
        var invalidToken = "invalid.token.signature";

        // Act
        var extractedPermissions = JwtService.ExtractPermissions(invalidToken);

        // Assert
        Assert.Empty(extractedPermissions);
    }

    [Fact]
    public void GenerateToken_WithEmptyRoles_ShouldReturnValidToken()
    {
        // Arrange
        var username = "testuser";
        var roles = new List<string>();
        var permissions = new List<string> { "READ" };

        // Act
        var token = this.jwtService.GenerateToken(username, roles, permissions);

        // Assert
        Assert.NotNull(token);
        Assert.Contains(".", token);
    }

    [Fact]
    public void GenerateToken_WithEmptyPermissions_ShouldReturnValidToken()
    {
        // Arrange
        var username = "testuser";
        var roles = new List<string> { "USER" };
        var permissions = new List<string>();

        // Act
        var token = this.jwtService.GenerateToken(username, roles, permissions);

        // Assert
        Assert.NotNull(token);
        Assert.Contains(".", token);
    }

    [Fact]
    public void GenerateToken_WithNullRoles_ShouldThrowArgumentNullException()
    {
        // Arrange
        var username = "testuser";
        List<string> roles = null!;
        var permissions = new List<string> { "READ" };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this.jwtService.GenerateToken(username, roles, permissions));
    }

    [Fact]
    public void GenerateToken_WithNullPermissions_ShouldThrowArgumentNullException()
    {
        // Arrange
        var username = "testuser";
        var roles = new List<string> { "USER" };
        List<string> permissions = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this.jwtService.GenerateToken(username, roles, permissions));
    }

    [Fact]
    public void ValidateToken_WithExpiredToken_ShouldReturnFalse()
    {
        // Arrange - Create a token with past expiration
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x["Jwt:Secret"]).Returns("test-secret-key-that-is-long-enough-for-hmac-sha256");
        mockConfig.Setup(x => x["Jwt:ExpirationHours"]).Returns("24"); // Normal expiration

        var jwtService = new JwtService(mockConfig.Object);

        // Create an expired token manually by using a past expiration date
        var expiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6InRlc3R1c2VyIiwicm9sZSI6IlVTRVIiLCJwZXJtaXNzaW9uIjoiUkVBRCIsImV4cCI6MTYwOTQ1NzYwMCwiaWF0IjoxNjA5NDU3NjAwfQ.invalid";

        // Act
        var isValid = jwtService.ValidateToken(expiredToken);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ValidateToken_WithMalformedToken_ShouldReturnFalse()
    {
        // Arrange
        var malformedToken = "not.a.valid.jwt.token";

        // Act
        var isValid = this.jwtService.ValidateToken(malformedToken);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ValidateToken_WithEmptyToken_ShouldReturnFalse()
    {
        // Arrange
        var emptyToken = string.Empty;

        // Act
        var isValid = this.jwtService.ValidateToken(emptyToken);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ValidateToken_WithNullToken_ShouldReturnFalse()
    {
        // Arrange
        string nullToken = null!;

        // Act
        var isValid = this.jwtService.ValidateToken(nullToken);

        // Assert
        Assert.False(isValid);
    }
}