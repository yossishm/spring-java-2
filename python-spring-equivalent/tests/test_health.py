"""
Tests for health check endpoints.
"""

import pytest
from fastapi.testclient import TestClient
from unittest.mock import patch, AsyncMock

from python_spring_equivalent.main import app

client = TestClient(app)


class TestHealthEndpoints:
    """Test cases for health endpoints."""
    
    def test_readiness_probe_success(self):
        """Test successful readiness probe."""
        with patch('python_spring_equivalent.services.cache_service.health_check', new_callable=AsyncMock) as mock_cache, \
             patch('python_spring_equivalent.services.database_service.health_check', new_callable=AsyncMock) as mock_db:
            
            mock_cache.return_value = {"status": "healthy"}
            mock_db.return_value = {"status": "healthy"}
            
            response = client.get("/health/ready")
            assert response.status_code == 200
            data = response.json()
            assert data["status"] == "healthy"
            assert "timestamp" in data
            assert "version" in data
            assert "uptime" in data
    
    def test_readiness_probe_unhealthy(self):
        """Test readiness probe when services are unhealthy."""
        with patch('python_spring_equivalent.services.cache_service.health_check', new_callable=AsyncMock) as mock_cache, \
             patch('python_spring_equivalent.services.database_service.health_check', new_callable=AsyncMock) as mock_db:
            
            mock_cache.return_value = {"status": "unhealthy", "error": "Connection failed"}
            mock_db.return_value = {"status": "healthy"}
            
            response = client.get("/health/ready")
            assert response.status_code == 200
            data = response.json()
            assert data["status"] == "unhealthy"
    
    def test_liveness_probe(self):
        """Test liveness probe."""
        response = client.get("/health/live")
        assert response.status_code == 200
        data = response.json()
        assert data["status"] == "healthy"
        assert "timestamp" in data
        assert "version" in data
        assert "uptime" in data
    
    def test_actuator_health_success(self):
        """Test Spring Actuator compatible health check."""
        with patch('python_spring_equivalent.services.cache_service.health_check', new_callable=AsyncMock) as mock_cache, \
             patch('python_spring_equivalent.services.database_service.health_check', new_callable=AsyncMock) as mock_db:
            
            mock_cache.return_value = {"status": "healthy", "backend": "redis"}
            mock_db.return_value = {"status": "healthy", "backend": "mongodb"}
            
            response = client.get("/actuator/health")
            assert response.status_code == 200
            data = response.json()
            assert data["status"] == "UP"
            assert "components" in data
            assert "cache" in data["components"]
            assert "database" in data["components"]
            assert data["components"]["cache"]["status"] == "HEALTHY"
            assert data["components"]["database"]["status"] == "HEALTHY"
    
    def test_actuator_health_unhealthy(self):
        """Test Spring Actuator health check when unhealthy."""
        with patch('python_spring_equivalent.services.cache_service.health_check', new_callable=AsyncMock) as mock_cache, \
             patch('python_spring_equivalent.services.database_service.health_check', new_callable=AsyncMock) as mock_db:
            
            mock_cache.return_value = {"status": "unhealthy", "error": "Connection failed"}
            mock_db.return_value = {"status": "unhealthy", "error": "Database unavailable"}
            
            response = client.get("/actuator/health")
            assert response.status_code == 200
            data = response.json()
            assert data["status"] == "DOWN"
            assert data["components"]["cache"]["status"] == "UNHEALTHY"
            assert data["components"]["database"]["status"] == "UNHEALTHY"
    
    def test_application_info(self):
        """Test application info endpoint."""
        response = client.get("/actuator/info")
        assert response.status_code == 200
        data = response.json()
        assert "app" in data
        assert "build" in data
        assert "system" in data
        assert "security" in data
        assert data["app"]["name"] == "Python Spring Equivalent"
        assert "version" in data["app"]


class TestHealthMetrics:
    """Test health-related metrics."""
    
    def test_health_metrics_endpoint(self):
        """Test health metrics endpoint."""
        response = client.get("/metrics/health")
        assert response.status_code == 200
        data = response.json()
        assert "status" in data
        assert "metrics" in data
        assert "timestamp" in data
        assert "error_rate" in data["metrics"]
        assert "cache_efficiency" in data["metrics"]
    
    def test_metrics_endpoint(self):
        """Test general metrics endpoint."""
        response = client.get("/metrics/")
        assert response.status_code == 200
        data = response.json()
        assert "metrics" in data
        assert "timestamp" in data
        assert "uptime_seconds" in data["metrics"]
        assert "request_count" in data["metrics"]
    
    def test_prometheus_metrics(self):
        """Test Prometheus metrics format."""
        response = client.get("/metrics/prometheus")
        assert response.status_code == 200
        content = response.text
        assert "python_spring_equivalent_requests_total" in content
        assert "python_spring_equivalent_errors_total" in content
        assert "python_spring_equivalent_uptime_seconds" in content
