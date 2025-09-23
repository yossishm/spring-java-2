namespace SpringJavaEquivalent.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using SpringJavaEquivalent.Controllers;
    using SpringJavaEquivalent.Services;
    using Xunit;

    public class TokenControllerTests
    {
        private readonly Mock<JwtService> mockJwtService;
        private readonly TokenController controller;

        public TokenControllerTests()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["Jwt:Secret"]).Returns("test-secret-key-that-is-long-enough-for-hmac-sha256");
            mockConfig.Setup(x => x["Jwt:ExpirationHours"]).Returns("24");

            this.mockJwtService = new Mock<JwtService>(mockConfig.Object);
            this.controller = new TokenController(this.mockJwtService.Object);
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
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult!.Value);
        }

        [Fact]
        public void GeneratePredefinedToken_WithUserType_ShouldReturnOkResult()
        {
            // Arrange
            var type = "user";

            // Act
            var result = this.controller.GeneratePredefinedToken(type);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult!.Value);
        }

        [Fact]
        public void GeneratePredefinedToken_WithInvalidType_ShouldReturnBadRequest()
        {
            // Arrange
            var type = "invalid";

            // Act
            var result = this.controller.GeneratePredefinedToken(type);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("Invalid token type. Use 'admin' or 'user'.", badRequestResult!.Value);
        }

        [Fact]
        public void GenerateAdminToken_ShouldReturnOkResult()
        {
            // Act
            var result = this.controller.GenerateAdminToken();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult!.Value);
        }

        [Fact]
        public void GenerateUserToken_ShouldReturnOkResult()
        {
            // Act
            var result = this.controller.GenerateUserToken();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult!.Value);
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
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult!.Value);
        }

        [Fact]
        public void ValidateToken_WithInvalidToken_ShouldReturnOkResultWithFalse()
        {
            // Arrange
            var request = new Dictionary<string, string> { { "token", "invalid-token" } };

            // Act
            var result = this.controller.ValidateToken(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult!.Value);
        }

        [Fact]
        public void ValidateToken_WithNullRequest_ShouldReturnBadRequest()
        {
            // Arrange
            Dictionary<string, string> request = null!;

            // Act
            var result = this.controller.ValidateToken(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("Request is required", badRequestResult!.Value);
        }

        [Fact]
        public void ValidateToken_WithEmptyToken_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new Dictionary<string, string> { { "token", string.Empty } };

            // Act
            var result = this.controller.ValidateToken(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("Token is required", badRequestResult!.Value);
        }
    }
}