# Python Spring Equivalent - FastAPI Application

This is a Python FastAPI equivalent of the Spring Boot Java application with comprehensive JWT-based authorization, cache services, OpenTelemetry tracing, and health checks.

## Features

- **FastAPI Framework**: Modern, fast web framework for building APIs
- **JWT Authentication & Authorization**: 8 security levels matching Spring Boot capabilities
- **Cache Service Endpoints**: GET, PUT, DELETE operations at `/api/v1/cacheServices/`
- **JWT Controller**: Token creation, verification, and vulnerable endpoints for security testing
- **Health Checks**: Multiple health check endpoints compatible with Spring Boot Actuator
- **OpenTelemetry**: Distributed tracing and metrics collection
- **Docker Support**: Multiple Docker configurations including distroless
- **Comprehensive Testing**: Unit and integration tests with pytest
- **Code Quality**: Black, isort, flake8, mypy, bandit, and safety tools

## Authorization Levels (Python vs Spring)

| Level | Description | Python ✅ | Spring ✅ |
|-------|-------------|-----------|-----------|
| 0 | Public Access | ✅ | ✅ |
| 1 | Basic Authentication | ✅ | ✅ |
| 2 | Role-Based Access | ✅ | ✅ |
| 3 | Admin Role Access | ✅ | ✅ |
| 4 | Permission-Based Access | ✅ | ✅ |
| 5 | Authentication Level Access | ✅ | ✅ |
| 6 | Identity Provider Access | ✅ | ✅ |
| 7 | Multi-Factor Authorization | ✅ | ✅ |

## Available Permissions

- `CACHE_READ` - Read cache operations
- `CACHE_WRITE` - Write cache operations
- `CACHE_DELETE` - Delete cache operations
- `CACHE_ADMIN` - Administrative cache operations

## Available Roles

- `USER` - Basic user access
- `ADMIN` - Administrative access
- `MANAGER` - Management access
- `READONLY` - Read-only access
- `WRITER` - Write access

## Authentication Levels

- `AAL1` - Basic authentication
- `AAL2` - Multi-factor authentication
- `AAL3` - Hardware-based authentication

## Identity Providers

- `local` - Local authentication
- `enterprise` - Enterprise SSO
- `external` - External provider

## Quick Start

### Prerequisites

- Python 3.11+
- Redis (for caching)
- MongoDB (for database)
- Docker (optional)

### Local Development

1. **Clone and setup**:
   ```bash
   cd python-spring-equivalent
   python -m venv venv
   source venv/bin/activate  # On Windows: venv\Scripts\activate
   pip install -r requirements.txt
   ```

2. **Configure environment**:
   ```bash
   cp env.example .env
   # Edit .env with your configuration
   ```

3. **Start services**:
   ```bash
   # Start Redis and MongoDB
   docker run -d --name redis -p 6379:6379 redis:7.2-alpine
   docker run -d --name mongodb -p 27017:27017 mongo:7.0
   ```

4. **Run the application**:
   ```bash
   python -m uvicorn python_spring_equivalent.main:app --reload
   ```

5. **Access the application**:
   - API: http://localhost:8080
   - Documentation: http://localhost:8080/docs
   - Health Check: http://localhost:8080/health/ready

### Docker Deployment

1. **Build and run**:
   ```bash
   docker build -t python-spring-equivalent .
   docker run -p 8080:8080 python-spring-equivalent
   ```

2. **Using Docker Compose**:
   ```bash
   docker-compose up -d
   ```

3. **Distroless build**:
   ```bash
   docker build -f Dockerfile.distroless -t python-spring-equivalent-distroless .
   ```

## API Endpoints

### Main Application
- `GET /` - Home endpoint
- `GET /api/v1/cacheServices/getObject?id={id}` - Get cache object
- `PUT /api/v1/cacheServices/putObject?id={id}` - Put cache object
- `DELETE /api/v1/cacheServices/deleteObject?id={id}` - Delete cache object

### JWT Operations
- `POST /api/jwt/create` - Create JWT token
- `POST /api/jwt/verify` - Verify JWT token
- `GET /api/jwt/decode?token={token}` - Decode JWT token
- `POST /api/jwt/verify-any-algorithm` - Verify with any algorithm (VULNERABLE)
- `POST /api/jwt/create-vulnerable` - Create vulnerable token (VULNERABLE)

### Health Checks
- `GET /health/ready` - Readiness probe
- `GET /health/live` - Liveness probe
- `GET /actuator/health` - Spring Actuator compatible health check
- `GET /actuator/info` - Application information

### Metrics
- `GET /metrics/` - Application metrics
- `GET /metrics/prometheus` - Prometheus metrics
- `GET /metrics/health` - Health metrics

## Testing

### Run Tests
```bash
# Run all tests
pytest

# Run with coverage
pytest --cov=python_spring_equivalent --cov-report=html

# Run specific test file
pytest tests/test_cache.py

# Run with verbose output
pytest -v
```

### Test Examples

```bash
# Test cache operations
curl -H "Authorization: Bearer YOUR_TOKEN" \
  "http://localhost:8080/api/v1/cacheServices/getObject?id=test123"

# Test JWT creation
curl -X POST "http://localhost:8080/api/jwt/create" \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "password"}'

# Test health check
curl "http://localhost:8080/health/ready"
```

## Code Quality

### Formatting and Linting
```bash
# Format code
black python_spring_equivalent/ tests/

# Sort imports
isort python_spring_equivalent/ tests/

# Lint code
flake8 python_spring_equivalent/ tests/

# Type checking
mypy python_spring_equivalent/

# Security check
bandit -r python_spring_equivalent/
safety check
```

### Pre-commit Hooks
```bash
# Install pre-commit
pip install pre-commit

# Install hooks
pre-commit install

# Run hooks manually
pre-commit run --all-files
```

## Monitoring and Observability

### OpenTelemetry
The application includes OpenTelemetry instrumentation for:
- Distributed tracing
- Metrics collection
- Logging correlation

Configure OTLP endpoint:
```bash
export OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:14250
```

### Prometheus Metrics
Access Prometheus metrics at `/metrics/prometheus` for monitoring and alerting.

### Jaeger Tracing
View distributed traces in Jaeger UI at http://localhost:16686

## Security Features

### JWT Security
- Strong secret key validation
- Algorithm verification
- Token expiration
- Permission-based authorization
- Role-based access control

### Vulnerable Endpoints (Testing Only)
- `/api/jwt/verify-any-algorithm` - Accepts any algorithm
- `/api/jwt/create-vulnerable` - Creates weak tokens

**⚠️ WARNING**: These endpoints are intentionally vulnerable for security testing. Do not use in production!

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `DEBUG` | Enable debug mode | `false` |
| `HOST` | Server host | `0.0.0.0` |
| `PORT` | Server port | `8080` |
| `JWT_SECRET` | JWT signing secret | Required |
| `JWT_ALGORITHM` | JWT algorithm | `HS256` |
| `JWT_EXPIRATION_HOURS` | Token expiration | `24` |
| `MONGODB_URL` | MongoDB connection | `mongodb://localhost:27017` |
| `REDIS_URL` | Redis connection | `redis://localhost:6379` |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | OTLP endpoint | Optional |

## Project Structure

```
python-spring-equivalent/
├── python_spring_equivalent/
│   ├── __init__.py
│   ├── main.py                 # FastAPI application
│   ├── config.py               # Configuration management
│   ├── security.py             # JWT authentication
│   ├── models.py               # Pydantic models
│   ├── services.py             # Business logic services
│   ├── telemetry.py            # OpenTelemetry setup
│   └── routers/
│       ├── __init__.py
│       ├── cache.py            # Cache endpoints
│       ├── jwt.py              # JWT endpoints
│       ├── health.py           # Health check endpoints
│       └── metrics.py          # Metrics endpoints
├── tests/
│   ├── __init__.py
│   ├── test_main.py            # Main app tests
│   ├── test_cache.py           # Cache tests
│   ├── test_jwt.py             # JWT tests
│   └── test_health.py          # Health check tests
├── requirements.txt            # Python dependencies
├── pyproject.toml              # Project configuration
├── Dockerfile                  # Standard Docker build
├── Dockerfile.distroless       # Distroless Docker build
├── docker-compose.yml          # Multi-service setup
├── prometheus.yml              # Prometheus configuration
└── README.md                   # This file
```

## Comparison with Spring Boot

This Python FastAPI application provides equivalent functionality to the Spring Boot Java application:

| Feature | Spring Boot | Python FastAPI |
|---------|-------------|----------------|
| Web Framework | Spring Boot | FastAPI |
| Authentication | Spring Security | python-jose + custom |
| Authorization | @PreAuthorize | Decorators |
| Health Checks | Actuator | Custom endpoints |
| Metrics | Micrometer | OpenTelemetry |
| Documentation | Swagger/OpenAPI | FastAPI auto-docs |
| Testing | JUnit 5 | pytest |
| Docker | Multi-stage | Multi-stage |
| Caching | Spring Cache | Redis |
| Database | Spring Data | Motor (MongoDB) |

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Run code quality checks
6. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Security Note

The JWT controller contains intentionally vulnerable code for security testing purposes. Do not use in production without proper security measures.
