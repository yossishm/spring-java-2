# Spring Java Equivalent - .NET 8 Project

This is a .NET 8 equivalent of the Spring Java application with all the same endpoints, health checks, and Docker configurations.

## Features

- **Cache Service Endpoints**: GET, PUT, DELETE operations at `/api/v1/cacheServices/`
- **JWT Controller**: Vulnerable JWT endpoints at `/api/jwt/` (for security testing)
- **Health Checks**: Multiple health check endpoints
- **OpenTelemetry**: Distributed tracing and metrics
- **Docker Support**: Multiple Docker configurations

## Endpoints

### Main Application
- `GET /` - Home endpoint
- `GET /api/v1/cacheServices/getObject?id={id}` - Get cache object
- `PUT /api/v1/cacheServices/putObject?id={id}` - Put cache object
- `DELETE /api/v1/cacheServices/deleteObject?id={id}` - Delete cache object

### JWT Controller (Vulnerable - for testing)
- `POST /api/jwt/create` - Create vulnerable JWT token
- `POST /api/jwt/verify` - Verify JWT token (no proper validation)
- `GET /api/jwt/decode?token={token}` - Decode JWT token
- `POST /api/jwt/verify-any-algorithm` - Verify with any algorithm

### Health Checks
- `GET /health/ready` - Readiness probe
- `GET /health/live` - Liveness probe
- `GET /actuator/health` - Spring Actuator compatible health check

## Running the Application

### Local Development
```bash
cd dotnet-spring-equivalent
dotnet run
```

### Docker
```bash
# Standard Docker build
docker build -t spring-java-equivalent .

# Distroless build
docker build -f Dockerfile.distroless -t spring-java-equivalent-distroless .

# Native AOT build
docker build -f Dockerfile.native -t spring-java-equivalent-native .

# Secure build
docker build -f Dockerfile.secure -t spring-java-equivalent-secure .

# Scratch build (minimal)
docker build -f Dockerfile.scratch -t spring-java-equivalent-scratch .
```

### Docker Compose
```bash
docker-compose up
```

This will start three instances:
- Standard: http://localhost:8080
- Distroless: http://localhost:8081
- Native: http://localhost:8082

## Testing Endpoints

### Cache Service
```bash
# Get object
curl "http://localhost:8080/api/v1/cacheServices/getObject?id=test123"

# Put object
curl -X PUT "http://localhost:8080/api/v1/cacheServices/putObject?id=test123"

# Delete object
curl -X DELETE "http://localhost:8080/api/v1/cacheServices/deleteObject?id=test123"
```

### JWT Endpoints
```bash
# Create token
curl -X POST "http://localhost:8080/api/jwt/create" \
  -H "Content-Type: application/json" \
  -d '{"user": "test", "role": "admin"}'

# Verify token
curl -X POST "http://localhost:8080/api/jwt/verify" \
  -H "Content-Type: application/json" \
  -d '{"token": "your-token-here"}'

# Decode token
curl "http://localhost:8080/api/jwt/decode?token=your-token-here"
```

### Health Checks
```bash
# Readiness
curl "http://localhost:8080/health/ready"

# Liveness
curl "http://localhost:8080/health/live"

# Spring Actuator compatible
curl "http://localhost:8080/actuator/health"
```

## Project Structure

```
dotnet-spring-equivalent/
├── Controllers/
│   ├── ApplicationController.cs      # Main application endpoints
│   └── VulnerableJWTController.cs    # JWT endpoints (vulnerable)
├── Services/
│   └── LocalRestClient.cs            # HTTP client service
├── Program.cs                        # Application entry point
├── SpringJavaEquivalent.csproj       # Project file
├── appsettings.json                  # Configuration
├── Dockerfile                        # Standard Docker build
├── Dockerfile.distroless             # Distroless build
├── Dockerfile.native                 # Native AOT build
├── Dockerfile.secure                 # Secure build
├── Dockerfile.scratch                # Scratch build
└── docker-compose.yml                # Multi-container setup
```

## Security Note

The JWT controller contains intentionally vulnerable code for security testing purposes. Do not use in production without proper security measures.
