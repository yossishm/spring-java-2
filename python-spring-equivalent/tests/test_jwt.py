"""
Tests for JWT endpoints.
"""

import pytest
from fastapi.testclient import TestClient
from jose import jwt

from python_spring_equivalent.main import app
from python_spring_equivalent.config import settings

client = TestClient(app)


class TestJWTEndpoints:
    """Test cases for JWT endpoints."""
    
    def test_create_token_success(self):
        """Test successful token creation."""
        token_request = {
            "username": "admin",
            "password": "password",
            "permissions": ["CACHE_READ", "CACHE_WRITE"],
            "roles": ["USER"]
        }
        
        response = client.post("/api/jwt/create", json=token_request)
        assert response.status_code == 200
        data = response.json()
        assert "access_token" in data
        assert data["token_type"] == "bearer"
        assert "expires_in" in data
    
    def test_create_token_invalid_credentials(self):
        """Test token creation with invalid credentials."""
        token_request = {
            "username": "nonexistent",
            "password": "wrongpassword"
        }
        
        response = client.post("/api/jwt/create", json=token_request)
        assert response.status_code == 401
        data = response.json()
        assert "Invalid credentials" in data["detail"]
    
    def test_verify_token_success(self):
        """Test successful token verification."""
        # First create a token
        token_request = {
            "username": "admin",
            "password": "password"
        }
        create_response = client.post("/api/jwt/create", json=token_request)
        token = create_response.json()["access_token"]
        
        # Then verify it
        verify_request = {"token": token}
        response = client.post("/api/jwt/verify", json=verify_request)
        assert response.status_code == 200
        data = response.json()
        assert data["valid"] is True
        assert data["username"] == "admin"
    
    def test_verify_token_invalid(self):
        """Test token verification with invalid token."""
        verify_request = {"token": "invalid.token.here"}
        response = client.post("/api/jwt/verify", json=verify_request)
        assert response.status_code == 200
        data = response.json()
        assert data["valid"] is False
    
    def test_decode_token_success(self):
        """Test successful token decoding."""
        # First create a token
        token_request = {
            "username": "admin",
            "password": "password"
        }
        create_response = client.post("/api/jwt/create", json=token_request)
        token = create_response.json()["access_token"]
        
        # Then decode it
        response = client.get(f"/api/jwt/decode?token={token}")
        assert response.status_code == 200
        data = response.json()
        assert "header" in data
        assert "payload" in data
        assert "signature" in data
        assert data["payload"]["sub"] == "admin"
    
    def test_decode_token_invalid_format(self):
        """Test token decoding with invalid format."""
        response = client.get("/api/jwt/decode?token=invalid")
        assert response.status_code == 400
        data = response.json()
        assert "Invalid token format" in data["detail"]
    
    def test_verify_any_algorithm_vulnerable(self):
        """Test the vulnerable verify endpoint."""
        # Create a token with weak security
        weak_token = jwt.encode(
            {"sub": "testuser", "roles": ["ADMIN"]},
            "weak-secret",
            algorithm="HS256"
        )
        
        verify_request = {"token": weak_token}
        response = client.post("/api/jwt/verify-any-algorithm", json=verify_request)
        assert response.status_code == 200
        data = response.json()
        assert data["valid"] is True
        assert data["username"] == "testuser"
    
    def test_create_vulnerable_token(self):
        """Test creating a vulnerable token."""
        vulnerable_request = {
            "user": "testuser",
            "role": "admin",
            "algorithm": "HS256"
        }
        
        response = client.post("/api/jwt/create-vulnerable", json=vulnerable_request)
        assert response.status_code == 200
        data = response.json()
        assert "token" in data
        assert data["algorithm"] == "HS256"
        assert data["vulnerable"] is True
        
        # Verify the token can be decoded
        token = data["token"]
        decoded = jwt.decode(token, "weak-secret", algorithms=["HS256"])
        assert decoded["sub"] == "testuser"
        assert "admin" in decoded["roles"]


class TestJWTSecurity:
    """Test JWT security features."""
    
    def test_token_expiration(self):
        """Test that tokens expire correctly."""
        # This would require modifying the token creation to use a very short expiration
        # For now, we'll test that the token structure includes expiration
        token_request = {
            "username": "admin",
            "password": "password"
        }
        response = client.post("/api/jwt/create", json=token_request)
        token = response.json()["access_token"]
        
        # Decode without verification to check expiration
        payload = jwt.get_unverified_claims(token)
        assert "exp" in payload
        assert "iat" in payload
    
    def test_token_contains_required_claims(self):
        """Test that tokens contain required claims."""
        token_request = {
            "username": "admin",
            "password": "password",
            "permissions": ["CACHE_READ"],
            "roles": ["USER"],
            "auth_level": "AAL2",
            "identity_provider": "enterprise"
        }
        response = client.post("/api/jwt/create", json=token_request)
        token = response.json()["access_token"]
        
        # Decode without verification to check claims
        payload = jwt.get_unverified_claims(token)
        assert payload["sub"] == "admin"
        assert "CACHE_READ" in payload["permissions"]
        assert "USER" in payload["roles"]
        assert payload["auth_level"] == "AAL2"
        assert payload["identity_provider"] == "enterprise"
