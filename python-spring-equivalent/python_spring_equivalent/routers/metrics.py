"""
Metrics router - equivalent to Spring Boot metrics endpoints.
"""

import logging
import time
from datetime import datetime
from typing import Dict, Any

from fastapi import APIRouter, Depends, Request
from fastapi.responses import JSONResponse

from ..config import settings
from ..models import MetricsResponse
from ..services import MetricsService

logger = logging.getLogger(__name__)

router = APIRouter()


@router.get(
    "/",
    response_model=MetricsResponse,
    summary="Get application metrics",
    description="Get comprehensive application metrics and statistics.",
    responses={
        200: {"description": "Metrics retrieved successfully"}
    }
)
async def get_metrics(request: Request) -> MetricsResponse:
    """
    Get application metrics.
    
    This endpoint provides comprehensive application metrics including
    performance, usage, and health statistics.
    """
    logger.info("Metrics requested")
    
    try:
        # Get metrics from service
        metrics_service = MetricsService()
        metrics = metrics_service.get_metrics()
        
        # Add additional system metrics
        import psutil
        process = psutil.Process()
        
        system_metrics = {
            "cpu_percent": process.cpu_percent(),
            "memory_percent": process.memory_percent(),
            "memory_info": {
                "rss": process.memory_info().rss,
                "vms": process.memory_info().vms
            },
            "num_threads": process.num_threads(),
            "create_time": process.create_time()
        }
        
        # Combine all metrics
        all_metrics = {
            **metrics,
            "system": system_metrics,
            "timestamp": time.time(),
            "service": "python-spring-equivalent"
        }
        
        return MetricsResponse(
            metrics=all_metrics,
            timestamp=datetime.utcnow().isoformat()
        )
        
    except Exception as e:
        logger.error(f"Error getting metrics: {e}")
        return MetricsResponse(
            metrics={"error": str(e)},
            timestamp=datetime.utcnow().isoformat()
        )


@router.get(
    "/prometheus",
    summary="Prometheus metrics",
    description="Get metrics in Prometheus format.",
    responses={
        200: {"description": "Prometheus metrics retrieved successfully"}
    }
)
async def prometheus_metrics(request: Request) -> str:
    """
    Get metrics in Prometheus format.
    
    This endpoint provides metrics in Prometheus exposition format.
    """
    logger.info("Prometheus metrics requested")
    
    try:
        # Get metrics from service
        metrics_service = MetricsService()
        metrics = metrics_service.get_metrics()
        
        # Format as Prometheus metrics
        prometheus_lines = []
        
        # Application metrics
        prometheus_lines.append(f"# HELP python_spring_equivalent_requests_total Total number of requests")
        prometheus_lines.append(f"# TYPE python_spring_equivalent_requests_total counter")
        prometheus_lines.append(f"python_spring_equivalent_requests_total {metrics['request_count']}")
        
        prometheus_lines.append(f"# HELP python_spring_equivalent_errors_total Total number of errors")
        prometheus_lines.append(f"# TYPE python_spring_equivalent_errors_total counter")
        prometheus_lines.append(f"python_spring_equivalent_errors_total {metrics['error_count']}")
        
        prometheus_lines.append(f"# HELP python_spring_equivalent_cache_hits_total Total number of cache hits")
        prometheus_lines.append(f"# TYPE python_spring_equivalent_cache_hits_total counter")
        prometheus_lines.append(f"python_spring_equivalent_cache_hits_total {metrics['cache_hits']}")
        
        prometheus_lines.append(f"# HELP python_spring_equivalent_cache_misses_total Total number of cache misses")
        prometheus_lines.append(f"# TYPE python_spring_equivalent_cache_misses_total counter")
        prometheus_lines.append(f"python_spring_equivalent_cache_misses_total {metrics['cache_misses']}")
        
        prometheus_lines.append(f"# HELP python_spring_equivalent_uptime_seconds Application uptime in seconds")
        prometheus_lines.append(f"# TYPE python_spring_equivalent_uptime_seconds gauge")
        prometheus_lines.append(f"python_spring_equivalent_uptime_seconds {metrics['uptime_seconds']}")
        
        prometheus_lines.append(f"# HELP python_spring_equivalent_requests_per_second Current requests per second")
        prometheus_lines.append(f"# TYPE python_spring_equivalent_requests_per_second gauge")
        prometheus_lines.append(f"python_spring_equivalent_requests_per_second {metrics['requests_per_second']}")
        
        prometheus_lines.append(f"# HELP python_spring_equivalent_cache_hit_ratio Cache hit ratio")
        prometheus_lines.append(f"# TYPE python_spring_equivalent_cache_hit_ratio gauge")
        prometheus_lines.append(f"python_spring_equivalent_cache_hit_ratio {metrics['cache_hit_ratio']}")
        
        return "\n".join(prometheus_lines)
        
    except Exception as e:
        logger.error(f"Error getting Prometheus metrics: {e}")
        return f"# ERROR: {str(e)}"


@router.get(
    "/health",
    response_model=Dict[str, Any],
    summary="Health metrics",
    description="Get health-related metrics and status.",
    responses={
        200: {"description": "Health metrics retrieved successfully"}
    }
)
async def health_metrics(request: Request) -> Dict[str, Any]:
    """
    Get health-related metrics.
    
    This endpoint provides health-specific metrics and status information.
    """
    logger.info("Health metrics requested")
    
    try:
        # Get basic metrics
        metrics = metrics_service.get_metrics()
        
        # Calculate health indicators
        error_rate = metrics['error_count'] / max(metrics['request_count'], 1)
        cache_efficiency = metrics['cache_hit_ratio']
        
        # Determine health status
        health_status = "healthy"
        if error_rate > 0.1:  # More than 10% error rate
            health_status = "degraded"
        if error_rate > 0.5:  # More than 50% error rate
            health_status = "unhealthy"
        
        return {
            "status": health_status,
            "metrics": {
                "error_rate": error_rate,
                "cache_efficiency": cache_efficiency,
                "uptime_seconds": metrics['uptime_seconds'],
                "request_count": metrics['request_count'],
                "error_count": metrics['error_count']
            },
            "thresholds": {
                "error_rate_warning": 0.1,
                "error_rate_critical": 0.5,
                "cache_efficiency_warning": 0.5
            },
            "timestamp": datetime.utcnow().isoformat()
        }
        
    except Exception as e:
        logger.error(f"Error getting health metrics: {e}")
        return {
            "status": "unhealthy",
            "error": str(e),
            "timestamp": datetime.utcnow().isoformat()
        }
