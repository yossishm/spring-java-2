"""
Service classes for the Python Spring Equivalent application.
"""

import json
import logging
import time
from typing import Any, Dict, Optional

import httpx
import redis
from motor.motor_asyncio import AsyncIOMotorClient

from .config import settings

logger = logging.getLogger(__name__)


class CacheService:
    """Cache service for managing cache operations."""
    
    def __init__(self):
        self.redis_client = redis.from_url(settings.redis_url)
        self.default_ttl = settings.cache_ttl_seconds
    
    async def get_object(self, object_id: str) -> Optional[Any]:
        """Get an object from cache."""
        try:
            cached_data = self.redis_client.get(f"cache:{object_id}")
            if cached_data:
                return json.loads(cached_data)
            return None
        except Exception as e:
            logger.error(f"Error getting object {object_id}: {e}")
            return None
    
    async def put_object(self, object_id: str, data: Any, ttl: Optional[int] = None) -> bool:
        """Put an object in cache."""
        try:
            ttl = ttl or self.default_ttl
            serialized_data = json.dumps(data)
            result = self.redis_client.setex(f"cache:{object_id}", ttl, serialized_data)
            return bool(result)
        except Exception as e:
            logger.error(f"Error putting object {object_id}: {e}")
            return False
    
    async def delete_object(self, object_id: str) -> bool:
        """Delete an object from cache."""
        try:
            result = self.redis_client.delete(f"cache:{object_id}")
            return bool(result)
        except Exception as e:
            logger.error(f"Error deleting object {object_id}: {e}")
            return False
    
    async def health_check(self) -> Dict[str, Any]:
        """Check cache service health."""
        try:
            # Test Redis connection
            self.redis_client.ping()
            return {
                "status": "healthy",
                "service": "cache",
                "backend": "redis",
                "connected": True
            }
        except Exception as e:
            logger.error(f"Cache health check failed: {e}")
            return {
                "status": "unhealthy",
                "service": "cache",
                "backend": "redis",
                "connected": False,
                "error": str(e)
            }


class DatabaseService:
    """Database service for MongoDB operations."""
    
    def __init__(self):
        self.client: Optional[AsyncIOMotorClient] = None
        self.database = None
    
    async def connect(self):
        """Connect to MongoDB."""
        try:
            self.client = AsyncIOMotorClient(settings.mongodb_url)
            self.database = self.client.get_database("python_spring_equivalent")
            # Test connection
            await self.client.admin.command('ping')
            logger.info("Connected to MongoDB")
        except Exception as e:
            logger.error(f"Failed to connect to MongoDB: {e}")
            raise
    
    async def disconnect(self):
        """Disconnect from MongoDB."""
        if self.client:
            self.client.close()
            logger.info("Disconnected from MongoDB")
    
    async def health_check(self) -> Dict[str, Any]:
        """Check database service health."""
        try:
            if not self.client:
                return {
                    "status": "unhealthy",
                    "service": "database",
                    "backend": "mongodb",
                    "connected": False,
                    "error": "Not connected"
                }
            
            # Test connection
            await self.client.admin.command('ping')
            return {
                "status": "healthy",
                "service": "database",
                "backend": "mongodb",
                "connected": True
            }
        except Exception as e:
            logger.error(f"Database health check failed: {e}")
            return {
                "status": "unhealthy",
                "service": "database",
                "backend": "mongodb",
                "connected": False,
                "error": str(e)
            }


class LocalRestClient:
    """Local REST client for making HTTP requests."""
    
    def __init__(self, authorization: str = ""):
        self.base_url = "http://localhost:8080"
        self.authorization = authorization
        self.headers = {
            "Content-Type": "application/json",
            "Accept": "*/*"
        }
        if authorization:
            self.headers["Authorization"] = f"Bearer {authorization}"
    
    async def get(self, uri: str) -> Dict[str, Any]:
        """Make a GET request."""
        async with httpx.AsyncClient(timeout=30.0) as client:
            try:
                response = await client.get(
                    f"{self.base_url}{uri}",
                    headers=self.headers,
                    timeout=30.0
                )
                return {
                    "status_code": response.status_code,
                    "data": response.json() if response.headers.get("content-type", "").startswith("application/json") else response.text,
                    "headers": dict(response.headers)
                }
            except Exception as e:
                logger.error(f"GET request failed for {uri}: {e}")
                return {
                    "status_code": 500,
                    "data": None,
                    "error": str(e)
                }
    
    async def post(self, uri: str, json_data: Dict[str, Any]) -> Dict[str, Any]:
        """Make a POST request."""
        async with httpx.AsyncClient(timeout=30.0) as client:
            try:
                response = await client.post(
                    f"{self.base_url}{uri}",
                    headers=self.headers,
                    json=json_data,
                    timeout=30.0
                )
                return {
                    "status_code": response.status_code,
                    "data": response.json() if response.headers.get("content-type", "").startswith("application/json") else response.text,
                    "headers": dict(response.headers)
                }
            except Exception as e:
                logger.error(f"POST request failed for {uri}: {e}")
                return {
                    "status_code": 500,
                    "data": None,
                    "error": str(e)
                }
    
    async def put(self, uri: str, json_data: Dict[str, Any]) -> Dict[str, Any]:
        """Make a PUT request."""
        async with httpx.AsyncClient(timeout=30.0) as client:
            try:
                response = await client.put(
                    f"{self.base_url}{uri}",
                    headers=self.headers,
                    json=json_data,
                    timeout=30.0
                )
                return {
                    "status_code": response.status_code,
                    "data": response.json() if response.headers.get("content-type", "").startswith("application/json") else response.text,
                    "headers": dict(response.headers)
                }
            except Exception as e:
                logger.error(f"PUT request failed for {uri}: {e}")
                return {
                    "status_code": 500,
                    "data": None,
                    "error": str(e)
                }
    
    async def delete(self, uri: str) -> Dict[str, Any]:
        """Make a DELETE request."""
        async with httpx.AsyncClient(timeout=30.0) as client:
            try:
                response = await client.delete(
                    f"{self.base_url}{uri}",
                    headers=self.headers,
                    timeout=30.0
                )
                return {
                    "status_code": response.status_code,
                    "data": response.json() if response.headers.get("content-type", "").startswith("application/json") else response.text,
                    "headers": dict(response.headers)
                }
            except Exception as e:
                logger.error(f"DELETE request failed for {uri}: {e}")
                return {
                    "status_code": 500,
                    "data": None,
                    "error": str(e)
                }


class MetricsService:
    """Service for collecting and exposing application metrics."""
    
    def __init__(self):
        self.start_time = time.time()
        self.request_count = 0
        self.error_count = 0
        self.cache_hits = 0
        self.cache_misses = 0
    
    def increment_request_count(self):
        """Increment request counter."""
        self.request_count += 1
    
    def increment_error_count(self):
        """Increment error counter."""
        self.error_count += 1
    
    def increment_cache_hit(self):
        """Increment cache hit counter."""
        self.cache_hits += 1
    
    def increment_cache_miss(self):
        """Increment cache miss counter."""
        self.cache_misses += 1
    
    def get_metrics(self) -> Dict[str, Any]:
        """Get current metrics."""
        uptime = time.time() - self.start_time
        return {
            "uptime_seconds": uptime,
            "request_count": self.request_count,
            "error_count": self.error_count,
            "cache_hits": self.cache_hits,
            "cache_misses": self.cache_misses,
            "cache_hit_ratio": self.cache_hits / max(self.cache_hits + self.cache_misses, 1),
            "requests_per_second": self.request_count / max(uptime, 1),
            "errors_per_second": self.error_count / max(uptime, 1)
        }
