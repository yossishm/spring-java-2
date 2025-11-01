"""
Main FastAPI application - Python Spring Equivalent.
"""

import logging
import time
from contextlib import asynccontextmanager
from typing import Dict, Any

from fastapi import FastAPI, HTTPException, Depends, Request, Response
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse
from fastapi.security import HTTPBearer
import structlog

from .config import settings
from .services import CacheService, DatabaseService, MetricsService
from .routers import cache, jwt, health, metrics
from .telemetry import setup_telemetry

# Configure structured logging
structlog.configure(
    processors=[
        structlog.stdlib.filter_by_level,
        structlog.stdlib.add_logger_name,
        structlog.stdlib.add_log_level,
        structlog.stdlib.PositionalArgumentsFormatter(),
        structlog.processors.TimeStamper(fmt="iso"),
        structlog.processors.StackInfoRenderer(),
        structlog.processors.format_exc_info,
        structlog.processors.UnicodeDecoder(),
        structlog.processors.JSONRenderer()
    ],
    context_class=dict,
    logger_factory=structlog.stdlib.LoggerFactory(),
    wrapper_class=structlog.stdlib.BoundLogger,
    cache_logger_on_first_use=True,
)

logger = structlog.get_logger()

# Global services
cache_service = CacheService()
database_service = DatabaseService()
metrics_service = MetricsService()


@asynccontextmanager
async def lifespan(app: FastAPI):
    """Application lifespan manager."""
    # Startup
    logger.info("Starting Python Spring Equivalent application")
    try:
        # Set up OpenTelemetry
        setup_telemetry()
        
        # Try to connect to database (optional for testing)
        try:
            await database_service.connect()
            logger.info("Database connected successfully")
        except Exception as db_error:
            logger.warning(f"Database connection failed (continuing without DB): {db_error}")
        
        logger.info("Application startup completed")
    except Exception as e:
        logger.error(f"Application startup failed: {e}")
        # Don't raise the exception to allow the app to continue without external dependencies
        logger.warning("Continuing without external dependencies for testing")
    
    yield
    
    # Shutdown
    logger.info("Shutting down Python Spring Equivalent application")
    await database_service.disconnect()
    logger.info("Application shutdown completed")


# Create FastAPI application
app = FastAPI(
    title=settings.app_name,
    version=settings.app_version,
    description="""
    ## Python Spring Equivalent API Documentation

    This API demonstrates comprehensive JWT-based authorization with **8 security levels** that match Spring capabilities:

    ### Authorization Levels (Python vs Spring):
    0. **Public Access** - No authentication required (Python ✅, Spring ✅)
    1. **Basic Authentication** - Valid JWT token needed (Python ✅, Spring ✅)
    2. **Role-Based Access** - Specific roles required (Python ✅, Spring ✅)
    3. **Admin Role Access** - ADMIN role required (Python ✅, Spring ✅)
    4. **Permission-Based Access** - Specific permissions required (Python ✅, Spring ✅)
    5. **Authentication Level Access** - AAL2+ required (Python ✅, Spring ✅)
    6. **Identity Provider Access** - Enterprise IDP required (Python ✅, Spring ✅)
    7. **Multi-Factor Authorization** - Complex combinations (Python ✅, Spring ✅)

    ### Available Permissions:
    - `CACHE_READ` - Read cache operations
    - `CACHE_WRITE` - Write cache operations
    - `CACHE_DELETE` - Delete cache operations
    - `CACHE_ADMIN` - Administrative cache operations

    ### Available Roles:
    - `USER` - Basic user access
    - `ADMIN` - Administrative access
    - `MANAGER` - Management access
    - `READONLY` - Read-only access
    - `WRITER` - Write access

    ### Authentication Levels:
    - `AAL1` - Basic authentication
    - `AAL2` - Multi-factor authentication
    - `AAL3` - Hardware-based authentication

    ### Identity Providers:
    - `local` - Local authentication
    - `enterprise` - Enterprise SSO
    - `external` - External provider
    """,
    lifespan=lifespan,
    docs_url="/docs" if settings.debug else None,
    redoc_url="/redoc" if settings.debug else None,
)

# Add CORS middleware
app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.cors_origins,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.middleware("http")
async def metrics_middleware(request: Request, call_next):
    """Middleware to collect metrics."""
    start_time = time.time()
    metrics_service.increment_request_count()
    
    try:
        response = await call_next(request)
        return response
    except Exception as e:
        metrics_service.increment_error_count()
        logger.error(f"Request failed: {e}")
        raise
    finally:
        # Log request metrics
        duration = time.time() - start_time
        logger.info(
            "Request completed",
            method=request.method,
            url=str(request.url),
            status_code=getattr(response, 'status_code', 500),
            duration=duration
        )


# Include routers
app.include_router(cache.router, prefix="/api/v1", tags=["Cache Services"])
app.include_router(jwt.router, prefix="/api/jwt", tags=["JWT Operations"])
app.include_router(health.router, prefix="/health", tags=["Health Checks"])
app.include_router(health.router, prefix="/actuator", tags=["Spring Actuator Compatible"])
app.include_router(metrics.router, prefix="/metrics", tags=["Metrics"])


@app.get("/", tags=["Root"])
async def home(request: Request) -> Dict[str, Any]:
    """
    Home endpoint - equivalent to Spring Boot root endpoint.
    
    Returns a simple greeting message.
    """
    logger.info("Home endpoint accessed")
    return {
        "message": "Hello Docker Python World",
        "version": settings.app_version,
        "service": "python-spring-equivalent"
    }


@app.exception_handler(HTTPException)
async def http_exception_handler(request: Request, exc: HTTPException) -> JSONResponse:
    """Global HTTP exception handler."""
    metrics_service.increment_error_count()
    logger.error(
        "HTTP exception occurred",
        status_code=exc.status_code,
        detail=exc.detail,
        url=str(request.url)
    )
    
    return JSONResponse(
        status_code=exc.status_code,
        content={
            "error": "HTTP Error",
            "message": exc.detail,
            "status_code": exc.status_code
        }
    )


@app.exception_handler(Exception)
async def general_exception_handler(request: Request, exc: Exception) -> JSONResponse:
    """Global exception handler."""
    metrics_service.increment_error_count()
    logger.error(
        "Unhandled exception occurred",
        exception=str(exc),
        url=str(request.url),
        exc_info=True
    )
    
    return JSONResponse(
        status_code=500,
        content={
            "error": "Internal Server Error",
            "message": "An unexpected error occurred",
            "status_code": 500
        }
    )


if __name__ == "__main__":
    import uvicorn
    
    uvicorn.run(
        "python_spring_equivalent.main:app",
        host=settings.host,
        port=settings.port,
        reload=settings.debug,
        log_level="info"
    )
