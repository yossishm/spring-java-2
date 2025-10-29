"""
Cache service router - equivalent to Spring Boot cache endpoints.
"""

import logging
from typing import Dict, Any

from fastapi import APIRouter, Depends, HTTPException, Query, Request
from fastapi.responses import JSONResponse

from ..models import CacheResponse
from ..security import require_permission, get_current_user, User
from ..services import CacheService, MetricsService

logger = logging.getLogger(__name__)

router = APIRouter()


@router.get(
    "/cacheServices/getObject",
    response_model=CacheResponse,
    summary="Get cache object",
    description="Retrieves an object from cache. Requires CACHE_READ or CACHE_ADMIN permission.",
    responses={
        200: {"description": "Object retrieved successfully"},
        401: {"description": "Unauthorized - Invalid or missing JWT token"},
        403: {"description": "Forbidden - Insufficient permissions (requires CACHE_READ or CACHE_ADMIN)"},
        404: {"description": "Object not found"}
    }
)
async def get_object(
    request: Request,
    id: str = Query(..., description="Cache object ID", example="123"),
    current_user: User = Depends(require_permission("CACHE_READ"))
) -> CacheResponse:
    """
    Get an object from cache.
    
    This endpoint requires CACHE_READ or CACHE_ADMIN permission.
    """
    logger.info(f"Get operation called for object ID: [REDACTED]")
    
    try:
        # Get object from cache
        cache_service = CacheService()
        cached_object = await cache_service.get_object(id)
        
        if cached_object is None:
            metrics_service = MetricsService()
            metrics_service.increment_cache_miss()
            return CacheResponse(
                success=False,
                message="Object not found",
                data=None
            )
        
        metrics_service = MetricsService()
        metrics_service.increment_cache_hit()
        return CacheResponse(
            success=True,
            message="Object retrieved successfully",
            data=cached_object
        )
        
    except Exception as e:
        logger.error(f"Error getting object {id}: {e}")
        metrics_service = MetricsService()
        metrics_service.increment_error_count()
        raise HTTPException(
            status_code=500,
            detail=f"Error retrieving object: {str(e)}"
        )


@router.put(
    "/cacheServices/putObject",
    response_model=CacheResponse,
    summary="Put cache object",
    description="Stores an object in cache. Requires CACHE_WRITE or CACHE_ADMIN permission.",
    responses={
        200: {"description": "Object stored successfully"},
        401: {"description": "Unauthorized - Invalid or missing JWT token"},
        403: {"description": "Forbidden - Insufficient permissions (requires CACHE_WRITE or CACHE_ADMIN)"},
        500: {"description": "Internal server error"}
    }
)
async def put_object(
    request: Request,
    id: str = Query(..., description="Cache object ID", example="123"),
    data: Dict[str, Any] = {"message": "Hello from Python Spring Equivalent"},
    current_user: User = Depends(require_permission("CACHE_WRITE"))
) -> CacheResponse:
    """
    Put an object in cache.
    
    This endpoint requires CACHE_WRITE or CACHE_ADMIN permission.
    """
    logger.info(f"Put operation called for object ID: [REDACTED]")
    
    try:
        # Store object in cache
        cache_service = CacheService()
        success = await cache_service.put_object(id, data)
        
        if success:
            return CacheResponse(
                success=True,
                message="Object stored successfully",
                data={"id": id, "stored": True}
            )
        else:
            raise HTTPException(
                status_code=500,
                detail="Failed to store object in cache"
            )
            
    except Exception as e:
        logger.error(f"Error putting object {id}: {e}")
        metrics_service = MetricsService()
        metrics_service.increment_error_count()
        raise HTTPException(
            status_code=500,
            detail=f"Error storing object: {str(e)}"
        )


@router.delete(
    "/cacheServices/deleteObject",
    response_model=CacheResponse,
    summary="Delete cache object",
    description="Removes an object from cache. Requires CACHE_DELETE or CACHE_ADMIN permission.",
    responses={
        200: {"description": "Object deleted successfully"},
        401: {"description": "Unauthorized - Invalid or missing JWT token"},
        403: {"description": "Forbidden - Insufficient permissions (requires CACHE_DELETE or CACHE_ADMIN)"},
        500: {"description": "Internal server error"}
    }
)
async def delete_object(
    request: Request,
    id: str = Query(..., description="Cache object ID", example="123"),
    current_user: User = Depends(require_permission("CACHE_DELETE"))
) -> CacheResponse:
    """
    Delete an object from cache.
    
    This endpoint requires CACHE_DELETE or CACHE_ADMIN permission.
    """
    logger.info(f"Delete operation called for object ID: [REDACTED]")
    
    try:
        # Delete object from cache
        cache_service = CacheService()
        success = await cache_service.delete_object(id)
        
        if success:
            return CacheResponse(
                success=True,
                message="Object deleted successfully",
                data={"id": id, "deleted": True}
            )
        else:
            return CacheResponse(
                success=False,
                message="Object not found or already deleted",
                data={"id": id, "deleted": False}
            )
            
    except Exception as e:
        logger.error(f"Error deleting object {id}: {e}")
        metrics_service = MetricsService()
        metrics_service.increment_error_count()
        raise HTTPException(
            status_code=500,
            detail=f"Error deleting object: {str(e)}"
        )
