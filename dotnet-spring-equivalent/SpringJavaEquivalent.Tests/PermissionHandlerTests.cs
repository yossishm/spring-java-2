// <copyright file="PermissionHandlerTests.cs" company="SpringJavaEquivalent">
// Copyright (c) SpringJavaEquivalent. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Tests
{
    using Microsoft.AspNetCore.Authorization;
    using SpringJavaEquivalent.Authorization;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Unit tests for the advanced permission handlers.
    /// </summary>
    public class AdvancedPermissionHandlerTests
    {
        /// <summary>
        /// Verifies that the AnyPermissionHandler succeeds when the user has at least one of the required permissions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AnyPermissionHandler_UserHasOnePermission_Succeeds()
        {
            // Arrange
            var requirement = new AnyPermissionRequirement("PermA", "PermB");
            var handler = new AnyPermissionHandler();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("permission", "PermA") }, "test"));
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }

        /// <summary>
        /// Verifies that the AllPermissionsHandler succeeds only when the user has all the required permissions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AllPermissionsHandler_UserHasAllPermissions_Succeeds()
        {
            // Arrange
            var requirement = new AllPermissionsRequirement("PermA", "PermB");
            var handler = new AllPermissionsHandler();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("permission", "PermA"), new Claim("permission", "PermB") }, "test"));
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }

        /// <summary>
        /// Verifies that the RoleHandler succeeds when the user has the specified role.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task RoleHandler_UserHasRole_Succeeds()
        {
            // Arrange
            var requirement = new RoleRequirement("Admin");
            var handler = new RoleHandler();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Role, "Admin") }, "test"));
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }

        /// <summary>
        /// Verifies that the IdentityProviderHandler succeeds when the user's IDP matches.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task IdentityProviderHandler_IdpMatches_Succeeds()
        {
            // Arrange
            var requirement = new IdentityProviderRequirement("TestIdp");
            var handler = new IdentityProviderHandler();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("idp", "TestIdp") }, "test"));
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }

        /// <summary>
        /// Verifies that the AuthLevelHandler succeeds when the user's auth level meets the requirement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AuthLevelHandler_AuthLevelMet_Succeeds()
        {
            // Arrange
            var requirement = new AuthLevelRequirement("High");
            var handler = new AuthLevelHandler();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("auth_level", "High") }, "test"));
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }
    }
}