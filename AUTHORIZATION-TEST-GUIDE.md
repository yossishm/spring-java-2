# Unified Authorization Test Guide

This guide explains how to test and compare the authorization implementations in both Spring and .NET applications.

## Overview

Both Spring and .NET applications now implement identical authorization capabilities with **8 security levels**:

1. **Level 0**: Public Access (No authentication required)
2. **Level 1**: Basic Authentication (Valid JWT token needed)
3. **Level 2**: Role-Based Access Control (RBAC)
4. **Level 3**: Permission-Based Access Control (PBAC)
5. **Level 4**: Service-Specific Authorization
6. **Level 5**: Authentication Assurance Level (AAL)
7. **Level 6**: Identity Provider Access
8. **Level 7**: Multi-Factor Authorization

## Test Scripts

### 1. Unified Test Suite (`test-unified-auth.sh`)

The comprehensive test suite that tests both applications with identical scenarios.

```bash
chmod +x test-unified-auth.sh
./test-unified-auth.sh
```

**Features:**
- Tests all 8 authorization levels
- Compares Spring vs .NET results side by side
- Tracks pass/fail statistics
- Tests authorization failures
- Validates token generation and validation
- Tests all token types

### 2. Quick Comparison (`compare-auth-implementations.sh`)

A quick comparison script for rapid verification.

```bash
chmod +x compare-auth-implementations.sh
./compare-auth-implementations.sh
```

**Features:**
- Quick side-by-side comparison
- Tests key authorization scenarios
- Minimal output for fast verification

## Prerequisites

### Start Both Applications

**Spring Application:**
```bash
cd /Users/yshmulev/dev/spring-java-2
mvn spring-boot:run
```
- Runs on: http://localhost:8080
- Swagger UI: http://localhost:8080/swagger-ui.html

**NET Application:**
```bash
cd /Users/yshmulev/dev/spring-java-2/dotnet-spring-equivalent
dotnet run
```
- Runs on: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger

## Test Scenarios

### 1. Token Generation

Both applications support the same token types:

| Token Type | Roles | Permissions | Auth Level | IDP |
|------------|-------|-------------|------------|-----|
| `admin` | ADMIN, USER | All cache permissions | AAL3 | enterprise-ldap |
| `user` | USER | CACHE_READ | AAL1 | local |
| `cache-admin` | USER | All cache permissions | AAL2 | azure-ad |
| `cache-writer` | USER | CACHE_READ, CACHE_WRITE | AAL2 | okta |
| `cache-reader` | USER | CACHE_READ | AAL1 | local |
| `aal1-user` | USER | CACHE_READ | AAL1 | local |
| `aal2-user` | USER | CACHE_READ, CACHE_WRITE | AAL2 | azure-ad |
| `aal3-user` | ADMIN | All cache permissions | AAL3 | enterprise-ldap |

### 2. Authorization Levels Tested

#### Level 0: Public Access
- **Spring**: `GET /api/v1/test/public`
- **NET**: `GET /api/v1/enhanced-test/public`
- **Expected**: 200 OK (no authentication required)

#### Level 1: Basic Authentication
- **Spring**: `GET /api/v1/test/protected`
- **NET**: `GET /api/v1/enhanced-test/authenticated`
- **Expected**: 200 OK (with valid token), 401 Unauthorized (without token)

#### Level 2: Role-Based Access
- **Spring**: `GET /api/v1/test/admin`
- **NET**: `GET /api/v1/enhanced-test/admin-only`
- **Expected**: 200 OK (with ADMIN role), 403 Forbidden (with USER role)

#### Level 3: Permission-Based Access
- **Spring**: `GET /api/v1/test/cache/read`
- **NET**: `GET /api/v1/enhanced-test/permission-based`
- **Expected**: 200 OK (with CACHE_READ permission), 403 Forbidden (without permission)

#### Level 4: Service-Specific Authorization
- **Spring**: `GET /api/v1/cacheServices/getObject`
- **NET**: `GET /api/v1/cacheServices/getObject`
- **Expected**: 200 OK (with CACHE_READ or CACHE_ADMIN), 403 Forbidden (without permission)

#### Level 5: Authentication Level Access
- **Spring**: `GET /api/v1/enhanced-test/aal2-required`
- **NET**: `GET /api/v1/enhanced-test/aal2-required`
- **Expected**: 200 OK (with AAL2+), 403 Forbidden (with AAL1)

#### Level 6: Identity Provider Access
- **Spring**: `GET /api/v1/enhanced-test/enterprise-only`
- **NET**: `GET /api/v1/enhanced-test/enterprise-only`
- **Expected**: 200 OK (with enterprise-ldap IDP), 403 Forbidden (with other IDP)

#### Level 7: Multi-Factor Authorization
- **Spring**: `GET /api/v1/enhanced-test/multi-factor`
- **NET**: `GET /api/v1/enhanced-test/multi-factor`
- **Expected**: 200 OK (with ADMIN role + CACHE_ADMIN permission + AAL3/enterprise-ldap), 403 Forbidden (missing any requirement)

### 3. Authorization Failures

The test suite verifies that both applications return the same HTTP status codes for authorization failures:

- **401 Unauthorized**: Missing or invalid JWT token
- **403 Forbidden**: Valid token but insufficient permissions/roles

## Expected Results

When both applications are running correctly, you should see:

```
🎉 Perfect Match! Both applications have identical authorization capabilities!

Spring Results:
  ✅ Passed: 25
  ❌ Failed: 0
  📈 Success Rate: 100%

.NET Results:
  ✅ Passed: 25
  ❌ Failed: 0
  📈 Success Rate: 100%
```

## Manual Testing

### Using Swagger UI

1. **Spring Swagger**: http://localhost:8080/swagger-ui.html
2. **NET Swagger**: http://localhost:5000/swagger

Both Swagger UIs include:
- JWT Bearer authentication configuration
- Comprehensive API documentation
- Interactive testing capabilities
- Security requirement specifications

### Using curl

Generate a token:
```bash
# Spring
curl -X GET "http://localhost:8080/api/v1/auth/token/admin"

# .NET
curl -X GET "http://localhost:5000/api/v1/auth/token/admin"
```

Use the token:
```bash
# Spring
curl -X GET "http://localhost:8080/api/v1/test/admin" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"

# .NET
curl -X GET "http://localhost:5000/api/v1/enhanced-test/admin-only" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## Troubleshooting

### Common Issues

1. **Connection Refused**: Make sure both applications are running
2. **401 Unauthorized**: Check if the JWT token is valid and not expired
3. **403 Forbidden**: Verify the user has the required roles/permissions
4. **404 Not Found**: Check if the endpoint URL is correct

### Debug Mode

Enable debug logging in both applications:

**Spring** (`application.properties`):
```properties
logging.level.org.springframework.security=DEBUG
logging.level.hello.security=DEBUG
```

**NET** (`appsettings.json`):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  }
}
```

## Implementation Comparison

| Feature | Spring | .NET | Status |
|---------|--------|------|--------|
| JWT Authentication | ✅ | ✅ | Identical |
| Role-Based Access Control | ✅ | ✅ | Identical |
| Permission-Based Access Control | ✅ | ✅ | Identical |
| Authentication Assurance Levels | ✅ | ✅ | Identical |
| Identity Provider Tracking | ✅ | ✅ | Identical |
| Multi-Factor Authorization | ✅ | ✅ | Identical |
| Custom Authorization Policies | ✅ | ✅ | Identical |
| Swagger Documentation | ✅ | ✅ | Identical |
| Error Handling | ✅ | ✅ | Identical |

## Conclusion

Both Spring and .NET applications now have **identical authorization capabilities** with comprehensive JWT-based security, role-based access control, permission-based access control, and advanced authorization features. The unified test suite ensures that both implementations behave consistently and provide the same level of security.
