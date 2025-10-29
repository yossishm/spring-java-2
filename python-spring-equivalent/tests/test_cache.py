"""
Tests for cache service endpoints.
"""

import pytest
from fastapi.testclient import TestClient
from unittest.mock import patch, AsyncMock

from python_spring_equivalent.main import app
from python_spring_equivalent.security import create_access_token

client = TestClient(app)


class TestCacheEndpoints:
    """Test cases for cache endpoints."""
    
    def create_auth_token(self, permissions=None, roles=None):
        """Create an authentication token for testing."""
        if permissions is None:
            permissions = ["CACHE_READ", "CACHE_WRITE", "CACHE_DELETE"]
        if roles is None:
            roles = ["USER"]
        
        token_data = {
            "sub": "testuser",
            "permissions": permissions,
            "roles": roles,
            "auth_level": "AAL1",
            "identity_provider": "local"
        }
        return create_access_token(token_data)
    
    def test_get_object_without_auth(self):
        """Test get object without authentication."""
        response = client.get("/api/v1/cacheServices/getObject?id=test123")
        assert response.status_code == 401
    
    def test_get_object_with_auth(self):
        """Test get object with authentication."""
        token = self.create_auth_token()
        headers = {"Authorization": f"Bearer {token}"}
        
        with patch('python_spring_equivalent.services.cache_service.get_object', new_callable=AsyncMock) as mock_get:
            mock_get.return_value = {"id": "test123", "data": "test data"}
            
            response = client.get("/api/v1/cacheServices/getObject?id=test123", headers=headers)
            assert response.status_code == 200
            data = response.json()
            assert data["success"] is True
            assert data["message"] == "Object retrieved successfully"
    
    def test_get_object_not_found(self):
        """Test get object when object is not found."""
        token = self.create_auth_token()
        headers = {"Authorization": f"Bearer {token}"}
        
        with patch('python_spring_equivalent.services.cache_service.get_object', new_callable=AsyncMock) as mock_get:
            mock_get.return_value = None
            
            response = client.get("/api/v1/cacheServices/getObject?id=test123", headers=headers)
            assert response.status_code == 200
            data = response.json()
            assert data["success"] is False
            assert data["message"] == "Object not found"
    
    def test_put_object_without_auth(self):
        """Test put object without authentication."""
        response = client.put("/api/v1/cacheServices/putObject?id=test123")
        assert response.status_code == 401
    
    def test_put_object_with_auth(self):
        """Test put object with authentication."""
        token = self.create_auth_token()
        headers = {"Authorization": f"Bearer {token}"}
        
        with patch('python_spring_equivalent.services.cache_service.put_object', new_callable=AsyncMock) as mock_put:
            mock_put.return_value = True
            
            response = client.put("/api/v1/cacheServices/putObject?id=test123", headers=headers)
            assert response.status_code == 200
            data = response.json()
            assert data["success"] is True
            assert data["message"] == "Object stored successfully"
    
    def test_delete_object_without_auth(self):
        """Test delete object without authentication."""
        response = client.delete("/api/v1/cacheServices/deleteObject?id=test123")
        assert response.status_code == 401
    
    def test_delete_object_with_auth(self):
        """Test delete object with authentication."""
        token = self.create_auth_token()
        headers = {"Authorization": f"Bearer {token}"}
        
        with patch('python_spring_equivalent.services.cache_service.delete_object', new_callable=AsyncMock) as mock_delete:
            mock_delete.return_value = True
            
            response = client.delete("/api/v1/cacheServices/deleteObject?id=test123", headers=headers)
            assert response.status_code == 200
            data = response.json()
            assert data["success"] is True
            assert data["message"] == "Object deleted successfully"
    
    def test_insufficient_permissions(self):
        """Test endpoints with insufficient permissions."""
        # Create token without required permissions
        token = self.create_auth_token(permissions=["CACHE_READ"], roles=["USER"])
        headers = {"Authorization": f"Bearer {token}"}
        
        # Try to access write endpoint
        response = client.put("/api/v1/cacheServices/putObject?id=test123", headers=headers)
        assert response.status_code == 403
        
        # Try to access delete endpoint
        response = client.delete("/api/v1/cacheServices/deleteObject?id=test123", headers=headers)
        assert response.status_code == 403
    
    def test_admin_bypass(self):
        """Test that admin role bypasses permission checks."""
        # Create token with admin role but no specific permissions
        token = self.create_auth_token(permissions=[], roles=["ADMIN"])
        headers = {"Authorization": f"Bearer {token}"}
        
        with patch('python_spring_equivalent.services.cache_service.put_object', new_callable=AsyncMock) as mock_put:
            mock_put.return_value = True
            
            # Admin should be able to access all endpoints
            response = client.put("/api/v1/cacheServices/putObject?id=test123", headers=headers)
            assert response.status_code == 200
