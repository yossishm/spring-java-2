namespace SpringJavaEquivalent.Tests
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using SpringJavaEquivalent.Controllers;
    using System.Security.Claims;
    using Xunit;

    public class EnhancedAuthTestControllerTests
    {
        private readonly EnhancedAuthTestController _controller;

        public EnhancedAuthTestControllerTests()
        {
            this._controller = new EnhancedAuthTestController();

            // Setup controller context
            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            this._controller.ControllerContext = context;
        }

        [Fact]
        public void PublicEndpoint_ShouldReturnOkResult()
        {
            // Act
            var result = this._controller.PublicEndpoint();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult!.Value);
        }

        [Fact]
        public void AuthenticatedEndpoint_WithAuthenticatedUser_ShouldReturnOkResult()
        {
            // Arrange
            this.SetupAuthenticatedUser();

            // Act
            var result = this._controller.AuthenticatedEndpoint();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult!.Value);
        }

        [Fact]
        public void UserRoleEndpoint_WithUserRole_ShouldReturnOkResult()
        {
            // Arrange
            this.SetupUserRole();

            // Act
            var result = this._controller.UserRoleEndpoint();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult!.Value);
        }

        [Fact]
        public void AdminRoleEndpoint_WithAdminRole_ShouldReturnOkResult()
        {
            // Arrange
            this.SetupAdminRole();

            // Act
            var result = this._controller.AdminRoleEndpoint();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult!.Value);
        }

        [Fact]
        public void CacheReadEndpoint_WithReadPermission_ShouldReturnOkResult()
        {
            // Arrange
            this.SetupReadPermission();

            // Act
            var result = this._controller.CacheReadEndpoint();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult!.Value);
        }

        [Fact]
        public void CacheWriteEndpoint_WithWritePermission_ShouldReturnOkResult()
        {
            // Arrange
            this.SetupWritePermission();

            // Act
            var result = this._controller.CacheWriteEndpoint();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult!.Value);
        }

        private void SetupAuthenticatedUser()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            this._controller.ControllerContext.HttpContext.User = principal;
        }

        private void SetupUserRole()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "USER")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            this._controller.ControllerContext.HttpContext.User = principal;
        }

        private void SetupAdminRole()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "ADMIN")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            this._controller.ControllerContext.HttpContext.User = principal;
        }

        private void SetupReadPermission()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim("permission", "CACHE_READ")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            this._controller.ControllerContext.HttpContext.User = principal;
        }

        private void SetupWritePermission()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim("permission", "CACHE_WRITE")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            this._controller.ControllerContext.HttpContext.User = principal;
        }
    }
}