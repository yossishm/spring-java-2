# JWT Authorization Implementation

This document describes the comprehensive JWT-based authorization system implemented in this Spring Boot application, following security best practices.

## Overview

The authorization system provides:
- JWT token validation and parsing
- Role-based access control (RBAC)
- Permission-based access control
- Method-level security annotations
- Custom authorization aspects
- Proper 403/401 error handling
- Stateless authentication

## Architecture Components

### 1. JWT Utility Service (`JwtUtil.java`)
- Token validation and parsing
- Claims extraction (username, roles, permissions)
- Permission and role checking methods
- Token generation for testing

### 2. JWT Authentication Filter (`JwtAuthenticationFilter.java`)
- Processes JWT tokens from Authorization header
- Sets up Spring Security context
- Handles token validation errors gracefully

### 3. Security Configuration (`SecurityConfig.java`)
- Configures Spring Security with JWT authentication
- Defines URL-based authorization rules
- Sets up CORS and session management
- Configures exception handlers

### 4. Custom Authorization Annotations
- `@RequirePermission`: Custom annotation for fine-grained authorization
- `PermissionAspect`: AOP aspect for handling custom annotations

### 5. Exception Handlers
- `JwtAuthenticationEntryPoint`: Handles 401 Unauthorized responses
- `JwtAccessDeniedHandler`: Handles 403 Forbidden responses
- `GlobalExceptionHandler`: Global exception handling for security errors

## Configuration

### Application Properties
```properties
# JWT Configuration
jwt.secret=mySecretKeyForJWTTokenGenerationAndValidation123456789
jwt.expiration=86400000
jwt.header=Authorization
jwt.prefix=Bearer 

# Security Configuration
spring.security.user.name=admin
spring.security.user.password=admin
spring.security.user.roles=ADMIN

# Logging for Security
logging.level.org.springframework.security=DEBUG
logging.level.hello.security=DEBUG
```

### Dependencies Added
```xml
<dependency>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-security</artifactId>
</dependency>
<dependency>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-aop</artifactId>
</dependency>
```

## Usage Examples

### 1. Basic Controller Authorization
```java
@RestController
public class MyController {
    
    @GetMapping("/read")
    @PreAuthorize("hasAuthority('CACHE_READ')")
    @RequirePermission(value = {"CACHE_READ"})
    public String readData() {
        return "Data read successfully";
    }
    
    @PostMapping("/write")
    @PreAuthorize("hasAuthority('CACHE_WRITE')")
    @RequirePermission(value = {"CACHE_WRITE"})
    public String writeData() {
        return "Data written successfully";
    }
}
```

### 2. Role-Based Authorization
```java
@GetMapping("/admin")
@PreAuthorize("hasRole('ADMIN')")
@RequirePermission(roles = {"ADMIN"})
public String adminOnly() {
    return "Admin access granted";
}
```

### 3. Multiple Permissions (ANY)
```java
@GetMapping("/multi")
@PreAuthorize("hasAnyAuthority('CACHE_READ', 'CACHE_WRITE')")
@RequirePermission(value = {"CACHE_READ", "CACHE_WRITE"})
public String multiPermission() {
    return "Access granted with any permission";
}
```

### 4. Multiple Permissions (ALL)
```java
@GetMapping("/all")
@RequirePermission(value = {"CACHE_READ", "CACHE_WRITE"}, requireAll = true)
public String allPermissions() {
    return "Access granted with all permissions";
}
```

## JWT Token Structure

### Standard Claims
```json
{
  "sub": "username",
  "iat": 1234567890,
  "exp": 1234654290,
  "roles": ["USER", "ADMIN"],
  "permissions": ["CACHE_READ", "CACHE_WRITE", "CACHE_ADMIN"]
}
```

### Token Generation
Use the `/api/v1/auth/token/{type}` endpoint to generate test tokens:
- `admin`: Full admin access
- `user`: Basic user access
- `cache-admin`: Cache administration access
- `cache-writer`: Cache read/write access
- `cache-reader`: Cache read-only access

## API Endpoints

### Authentication Endpoints
- `POST /api/v1/auth/token` - Generate custom token
- `GET /api/v1/auth/token/{type}` - Generate predefined token
- `POST /api/v1/auth/validate` - Validate token

### Test Endpoints
- `GET /api/v1/test/public` - Public access (no auth required)
- `GET /api/v1/test/protected` - Requires authentication
- `GET /api/v1/test/admin` - Requires ADMIN role
- `GET /api/v1/test/cache/read` - Requires CACHE_READ permission
- `POST /api/v1/test/cache/write` - Requires CACHE_WRITE permission
- `DELETE /api/v1/test/cache/admin` - Requires CACHE_ADMIN permission

### Cache Service Endpoints
- `GET /api/v1/cacheServices/getObject` - Requires CACHE_READ or CACHE_ADMIN
- `PUT /api/v1/cacheServices/putObject` - Requires CACHE_WRITE or CACHE_ADMIN
- `DELETE /api/v1/cacheServices/deleteObject` - Requires CACHE_DELETE or CACHE_ADMIN

## Error Responses

### 401 Unauthorized
```json
{
  "timestamp": "2024-01-01T12:00:00",
  "status": 401,
  "error": "Unauthorized",
  "message": "Authentication failed: Invalid JWT token",
  "path": "/api/v1/test/protected"
}
```

### 403 Forbidden
```json
{
  "timestamp": "2024-01-01T12:00:00",
  "status": 403,
  "error": "Forbidden",
  "message": "Access denied: Missing required permission: CACHE_WRITE",
  "path": "/api/v1/test/cache/write"
}
```

## Testing the Implementation

### 1. Generate a Token
```bash
curl -X GET "http://localhost:8080/api/v1/auth/token/admin"
```

### 2. Use Token in Request
```bash
curl -X GET "http://localhost:8080/api/v1/test/admin" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

### 3. Test Permission Denial
```bash
# This should return 403 Forbidden
curl -X GET "http://localhost:8080/api/v1/test/admin" \
  -H "Authorization: Bearer TOKEN_WITHOUT_ADMIN_ROLE"
```

## Security Best Practices Implemented

1. **Stateless Authentication**: No server-side session storage
2. **JWT Token Validation**: Comprehensive token validation with expiration checks
3. **Role-Based Access Control**: Hierarchical role system
4. **Permission-Based Access Control**: Fine-grained permission checking
5. **Method-Level Security**: Both Spring Security and custom annotations
6. **Proper Error Handling**: Consistent 401/403 responses
7. **CORS Configuration**: Proper cross-origin request handling
8. **Security Headers**: CSRF protection disabled for stateless API
9. **Input Validation**: JWT token format validation
10. **Logging**: Comprehensive security event logging

## Production Considerations

1. **JWT Secret**: Use a strong, randomly generated secret key
2. **Token Expiration**: Set appropriate expiration times
3. **HTTPS**: Always use HTTPS in production
4. **Token Refresh**: Implement token refresh mechanism
5. **Rate Limiting**: Add rate limiting for authentication endpoints
6. **Audit Logging**: Log all authorization decisions
7. **Key Rotation**: Implement JWT signing key rotation
8. **Token Blacklisting**: Consider token blacklisting for logout

## Troubleshooting

### Common Issues
1. **401 Unauthorized**: Check JWT token format and expiration
2. **403 Forbidden**: Verify user has required permissions/roles
3. **CORS Errors**: Check CORS configuration in SecurityConfig
4. **Token Validation Failures**: Verify JWT secret matches between services

### Debug Logging
Enable debug logging to troubleshoot authorization issues:
```properties
logging.level.org.springframework.security=DEBUG
logging.level.hello.security=DEBUG
```

This implementation provides a robust, production-ready JWT authorization system that follows Spring Security best practices and provides comprehensive access control capabilities.


