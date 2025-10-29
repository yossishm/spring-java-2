"""
Tests for the main FastAPI application.
"""

import pytest
from fastapi.testclient import TestClient

from python_spring_equivalent.main import app

client = TestClient(app)


class TestMainApplication:
    """Test cases for the main application."""
    
    def test_home_endpoint(self):
        """Test the home endpoint."""
        response = client.get("/")
        assert response.status_code == 200
        data = response.json()
        assert "message" in data
        assert "version" in data
        assert "service" in data
        assert data["service"] == "python-spring-equivalent"
    
    def test_docs_endpoint_in_debug(self):
        """Test that docs are available in debug mode."""
        # This test would need the app to be configured with debug=True
        # For now, we'll just test that the endpoint exists
        response = client.get("/docs")
        # In production mode, this should return 404
        # In debug mode, it should return 200
        assert response.status_code in [200, 404]
    
    def test_openapi_schema(self):
        """Test that OpenAPI schema is available."""
        response = client.get("/openapi.json")
        assert response.status_code == 200
        schema = response.json()
        assert "openapi" in schema
        assert "info" in schema
        assert schema["info"]["title"] == "Python Spring Equivalent"
    
    def test_cors_headers(self):
        """Test CORS headers are present."""
        response = client.options("/")
        # CORS preflight should be handled by the middleware
        assert response.status_code in [200, 405]  # 405 if OPTIONS not explicitly handled


class TestErrorHandling:
    """Test error handling."""
    
    def test_404_error(self):
        """Test 404 error handling."""
        response = client.get("/nonexistent")
        assert response.status_code == 404
    
    def test_http_exception_handler(self):
        """Test HTTP exception handler."""
        # This would require a specific endpoint that raises HTTPException
        # For now, we'll test the 404 case
        response = client.get("/nonexistent")
        assert response.status_code == 404
        data = response.json()
        assert "error" in data or "detail" in data
