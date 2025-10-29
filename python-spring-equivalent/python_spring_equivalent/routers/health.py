"""
Health check router - equivalent to Spring Boot health endpoints.
"""

import logging
import time
from datetime import datetime
from typing import Dict, Any

from fastapi import APIRouter, Depends, Request
from fastapi.responses import JSONResponse

from ..config import settings
from ..models import HealthCheckResponse
from ..services import CacheService, DatabaseService, MetricsService

logger = logging.getLogger(__name__)

router = APIRouter()

# Application start time
app_start_time = time.time()


@router.get(
    "/ready",
    response_model=HealthCheckResponse,
    summary="Readiness probe",
    description="Check if the application is ready to serve requests.",
    responses={
        200: {"description": "Application is ready"},
        503: {"description": "Application is not ready"}
    }
)
async def readiness_probe(request: Request) -> HealthCheckResponse:
    """
    Readiness probe endpoint.
    
    This endpoint checks if the application is ready to serve requests.
    It verifies that all critical services are available.
    """
    logger.info("Readiness probe requested")
    
    try:
        # Check cache service health
        cache_service = CacheService()
        cache_health = await cache_service.health_check()
        if cache_health["status"] != "healthy":
            logger.warning(f"Cache service unhealthy: {cache_health}")
        
        # Check database service health
        database_service = DatabaseService()
        db_health = await database_service.health_check()
        if db_health["status"] != "healthy":
            logger.warning(f"Database service unhealthy: {db_health}")
        
        # Determine overall health
        overall_healthy = (
            cache_health["status"] == "healthy" and
            db_health["status"] == "healthy"
        )
        
        uptime = time.time() - app_start_time
        
        return HealthCheckResponse(
            status="healthy" if overall_healthy else "unhealthy",
            timestamp=datetime.utcnow().isoformat(),
            version=settings.app_version,
            uptime=uptime
        )
        
    except Exception as e:
        logger.error(f"Readiness probe failed: {e}")
        return HealthCheckResponse(
            status="unhealthy",
            timestamp=datetime.utcnow().isoformat(),
            version=settings.app_version,
            uptime=time.time() - app_start_time
        )


@router.get(
    "/live",
    response_model=HealthCheckResponse,
    summary="Liveness probe",
    description="Check if the application is alive and running.",
    responses={
        200: {"description": "Application is alive"},
        503: {"description": "Application is not alive"}
    }
)
async def liveness_probe(request: Request) -> HealthCheckResponse:
    """
    Liveness probe endpoint.
    
    This endpoint checks if the application is alive and running.
    It performs basic health checks without external dependencies.
    """
    logger.info("Liveness probe requested")
    
    try:
        uptime = time.time() - app_start_time
        
        return HealthCheckResponse(
            status="healthy",
            timestamp=datetime.utcnow().isoformat(),
            version=settings.app_version,
            uptime=uptime
        )
        
    except Exception as e:
        logger.error(f"Liveness probe failed: {e}")
        return HealthCheckResponse(
            status="unhealthy",
            timestamp=datetime.utcnow().isoformat(),
            version=settings.app_version,
            uptime=time.time() - app_start_time
        )


@router.get(
    "/health",
    response_model=Dict[str, Any],
    summary="Spring Actuator compatible health check",
    description="Health check endpoint compatible with Spring Boot Actuator.",
    responses={
        200: {"description": "Health status retrieved successfully"},
        503: {"description": "Application is unhealthy"}
    }
)
async def actuator_health(request: Request) -> Dict[str, Any]:
    """
    Spring Actuator compatible health check.
    
    This endpoint provides health information in Spring Boot Actuator format.
    """
    logger.info("Actuator health check requested")
    
    try:
        # Check all services
        cache_service = CacheService()
        cache_health = await cache_service.health_check()
        database_service = DatabaseService()
        db_health = await database_service.health_check()
        
        # Determine overall status
        overall_status = "UP" if (
            cache_health["status"] == "healthy" and
            db_health["status"] == "healthy"
        ) else "DOWN"
        
        uptime = time.time() - app_start_time
        
        return {
            "status": overall_status,
            "components": {
                "cache": {
                    "status": cache_health["status"].upper(),
                    "details": cache_health
                },
                "database": {
                    "status": db_health["status"].upper(),
                    "details": db_health
                }
            },
            "info": {
                "version": settings.app_version,
                "uptime_seconds": uptime,
                "service": "python-spring-equivalent"
            }
        }
        
    except Exception as e:
        logger.error(f"Actuator health check failed: {e}")
        return {
            "status": "DOWN",
            "components": {},
            "info": {
                "version": settings.app_version,
                "uptime_seconds": time.time() - app_start_time,
                "service": "python-spring-equivalent",
                "error": str(e)
            }
        }


@router.get(
    "/info",
    response_model=Dict[str, Any],
    summary="Application information",
    description="Get application information and metadata.",
    responses={
        200: {"description": "Application information retrieved successfully"}
    }
)
async def application_info(request: Request) -> Dict[str, Any]:
    """
    Application information endpoint.
    
    This endpoint provides application metadata and configuration.
    """
    logger.info("Application info requested")
    
    uptime = time.time() - app_start_time
    
    return {
        "app": {
            "name": settings.app_name,
            "version": settings.app_version,
            "description": "Python FastAPI equivalent of Spring Boot Java application"
        },
        "build": {
            "version": settings.app_version,
            "time": datetime.utcnow().isoformat()
        },
        "system": {
            "uptime_seconds": uptime,
            "python_version": "3.11+",
            "framework": "FastAPI"
        },
        "security": {
            "jwt_algorithm": settings.jwt_algorithm,
            "cors_origins": settings.cors_origins
        }
    }
