"""
Pydantic models for the Python Spring Equivalent application.
"""

from typing import Any, Dict, List, Optional
from pydantic import BaseModel, Field


class CacheObject(BaseModel):
    """Cache object model."""
    id: str = Field(..., description="Cache object ID")
    data: Any = Field(..., description="Object data")
    ttl: Optional[int] = Field(None, description="Time to live in seconds")


class CacheResponse(BaseModel):
    """Cache operation response model."""
    success: bool = Field(..., description="Operation success status")
    message: str = Field(..., description="Response message")
    data: Optional[Any] = Field(None, description="Response data")


class JWTTokenRequest(BaseModel):
    """JWT token creation request model."""
    username: str = Field(..., description="Username")
    password: str = Field(..., description="Password")
    permissions: Optional[List[str]] = Field(default=[], description="User permissions")
    roles: Optional[List[str]] = Field(default=["USER"], description="User roles")
    auth_level: Optional[str] = Field(default="AAL1", description="Authentication level")
    identity_provider: Optional[str] = Field(default="local", description="Identity provider")


class JWTTokenResponse(BaseModel):
    """JWT token response model."""
    access_token: str = Field(..., description="JWT access token")
    token_type: str = Field(default="bearer", description="Token type")
    expires_in: int = Field(..., description="Token expiration time in seconds")


class JWTVerifyRequest(BaseModel):
    """JWT token verification request model."""
    token: str = Field(..., description="JWT token to verify")


class JWTVerifyResponse(BaseModel):
    """JWT token verification response model."""
    valid: bool = Field(..., description="Token validity")
    username: Optional[str] = Field(None, description="Username from token")
    permissions: List[str] = Field(default=[], description="Permissions from token")
    roles: List[str] = Field(default=[], description="Roles from token")
    auth_level: Optional[str] = Field(None, description="Authentication level")
    identity_provider: Optional[str] = Field(None, description="Identity provider")


class JWTDecodeRequest(BaseModel):
    """JWT token decode request model."""
    token: str = Field(..., description="JWT token to decode")


class JWTDecodeResponse(BaseModel):
    """JWT token decode response model."""
    header: Dict[str, Any] = Field(..., description="JWT header")
    payload: Dict[str, Any] = Field(..., description="JWT payload")
    signature: str = Field(..., description="JWT signature")


class HealthCheckResponse(BaseModel):
    """Health check response model."""
    status: str = Field(..., description="Health status")
    timestamp: str = Field(..., description="Check timestamp")
    version: str = Field(..., description="Application version")
    uptime: float = Field(..., description="Application uptime in seconds")


class ErrorResponse(BaseModel):
    """Error response model."""
    error: str = Field(..., description="Error type")
    message: str = Field(..., description="Error message")
    details: Optional[Dict[str, Any]] = Field(None, description="Additional error details")


class MetricsResponse(BaseModel):
    """Metrics response model."""
    metrics: Dict[str, Any] = Field(..., description="Application metrics")
    timestamp: str = Field(..., description="Metrics timestamp")


class VulnerableJWTRequest(BaseModel):
    """Vulnerable JWT creation request model."""
    user: str = Field(..., description="Username")
    role: str = Field(..., description="User role")
    algorithm: Optional[str] = Field(default="HS256", description="JWT algorithm")


class VulnerableJWTResponse(BaseModel):
    """Vulnerable JWT response model."""
    token: str = Field(..., description="Generated JWT token")
    algorithm: str = Field(..., description="Used algorithm")
    vulnerable: bool = Field(default=True, description="Whether the token is vulnerable")
