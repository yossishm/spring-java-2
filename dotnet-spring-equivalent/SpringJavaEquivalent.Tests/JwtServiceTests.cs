using Microsoft.Extensions.Configuration;
using Moq;
using SpringJavaEquivalent.Services;
using Xunit;

namespace SpringJavaEquivalent.Tests;

public class JwtServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(x => x["Jwt:Secret"]).Returns("test-secret-key-that-is-long-enough-for-hmac-sha256");
        _mockConfiguration.Setup(x => x["Jwt:ExpirationHours"]).Returns("24");
        
        _jwtService = new JwtService(_mockConfiguration.Object);
    }

    [Fact]
    public void GenerateToken_WithValidInputs_ShouldReturnValidToken()
    {
        // Arrange
        var username = "testuser";
        var roles = new List<string> { "USER", "ADMIN" };
        var permissions = new List<string> { "READ", "WRITE" };

        // Act
        var token = _jwtService.GenerateToken(username, roles, permissions);

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
        Assert.Throws<ArgumentNullException>(() => _jwtService.GenerateToken(null!, roles, permissions));
    }

    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var username = "testuser";
        var roles = new List<string> { "USER" };
        var permissions = new List<string> { "READ" };
        var token = _jwtService.GenerateToken(username, roles, permissions);

        // Act
        var isValid = _jwtService.ValidateToken(token);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var isValid = _jwtService.ValidateToken(invalidToken);

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
        var token = _jwtService.GenerateToken(username, roles, permissions);

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
        var token = _jwtService.GenerateToken(username, roles, permissions);

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
        var token = _jwtService.GenerateToken(username, roles, permissions);

        // Act
        var extractedPermissions = JwtService.ExtractPermissions(token);

        // Assert
        Assert.Equal(2, extractedPermissions.Count);
        Assert.Contains("READ", extractedPermissions);
        Assert.Contains("WRITE", extractedPermissions);
    }
}