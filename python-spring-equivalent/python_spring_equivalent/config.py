"""
Configuration management for the Python Spring Equivalent application.
"""

import os
from typing import Optional
from pydantic_settings import BaseSettings
from pydantic import Field


class Settings(BaseSettings):
    """Application settings."""
    
    # Application
    app_name: str = "Python Spring Equivalent"
    app_version: str = "1.0.0"
    debug: bool = Field(default=False, env="DEBUG")
    
    # Server
    host: str = Field(default="0.0.0.0", env="HOST")
    port: int = Field(default=8080, env="PORT")
    
    # JWT Configuration
    jwt_secret: str = Field(
        default="your-256-bit-secret-key-here-change-in-production",
        env="JWT_SECRET"
    )
    jwt_algorithm: str = Field(default="HS256", env="JWT_ALGORITHM")
    jwt_expiration_hours: int = Field(default=24, env="JWT_EXPIRATION_HOURS")
    
    # Database
    mongodb_url: str = Field(
        default="mongodb://localhost:27017",
        env="MONGODB_URL"
    )
    redis_url: str = Field(
        default="redis://localhost:6379",
        env="REDIS_URL"
    )
    
    # OpenTelemetry
    otel_service_name: str = Field(
        default="python-spring-equivalent",
        env="OTEL_SERVICE_NAME"
    )
    otel_service_version: str = Field(
        default="1.0.0",
        env="OTEL_SERVICE_VERSION"
    )
    otel_exporter_otlp_endpoint: Optional[str] = Field(
        default=None,
        env="OTEL_EXPORTER_OTLP_ENDPOINT"
    )
    
    # Security
    cors_origins: list[str] = Field(
        default=["http://localhost:3000", "http://localhost:8080"],
        env="CORS_ORIGINS"
    )
    
    # Cache
    cache_ttl_seconds: int = Field(default=3600, env="CACHE_TTL_SECONDS")
    
    class Config:
        """Pydantic config."""
        env_file = ".env"
        case_sensitive = False


# Global settings instance
settings = Settings()
