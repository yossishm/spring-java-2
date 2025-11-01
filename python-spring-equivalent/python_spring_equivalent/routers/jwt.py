"""
JWT operations router - equivalent to Spring Boot JWT endpoints.
"""

import json
import logging
from datetime import datetime, timedelta
from typing import Dict, Any

from fastapi import APIRouter, HTTPException, Depends, Request
from fastapi.responses import JSONResponse
from jose import jwt, JWTError

from ..config import settings
from ..models import (
    JWTTokenRequest, JWTTokenResponse, JWTVerifyRequest, JWTVerifyResponse,
    JWTDecodeRequest, JWTDecodeResponse, VulnerableJWTRequest, VulnerableJWTResponse
)
from ..security import authenticate_user, create_access_token, verify_token, get_current_user, User

logger = logging.getLogger(__name__)

router = APIRouter()


@router.post(
    "/create",
    response_model=JWTTokenResponse,
    summary="Create JWT token",
    description="Create a JWT access token for authentication.",
    responses={
        200: {"description": "Token created successfully"},
        401: {"description": "Invalid credentials"},
        500: {"description": "Internal server error"}
    }
)
async def create_token(
    request: Request,
    token_request: JWTTokenRequest
) -> JWTTokenResponse:
    """
    Create a JWT access token.
    
    This endpoint creates a JWT token with the specified permissions and roles.
    """
    logger.info(f"Token creation requested for user: {token_request.username}")
    
    try:
        # Authenticate user
        user = authenticate_user(token_request.username, token_request.password)
        if not user:
            raise HTTPException(
                status_code=401,
                detail="Invalid credentials"
            )
        
        # Create token data
        token_data = {
            "sub": user.username,
            "permissions": token_request.permissions or user.permissions,
            "roles": token_request.roles or user.roles,
            "auth_level": token_request.auth_level or user.auth_level,
            "identity_provider": token_request.identity_provider or user.identity_provider,
            "iat": datetime.utcnow(),
            "exp": datetime.utcnow() + timedelta(hours=settings.jwt_expiration_hours)
        }
        
        # Create access token
        access_token = create_access_token(token_data)
        
        return JWTTokenResponse(
            access_token=access_token,
            token_type="bearer",  # nosec B106 - This is JWT token type, not a password
            expires_in=settings.jwt_expiration_hours * 3600
        )
        
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Error creating token: {e}")
        raise HTTPException(
            status_code=500,
            detail=f"Error creating token: {str(e)}"
        )


@router.post(
    "/verify",
    response_model=JWTVerifyResponse,
    summary="Verify JWT token",
    description="Verify a JWT token and return its contents.",
    responses={
        200: {"description": "Token verified successfully"},
        401: {"description": "Invalid token"},
        500: {"description": "Internal server error"}
    }
)
async def verify_token_endpoint(
    request: Request,
    verify_request: JWTVerifyRequest
) -> JWTVerifyResponse:
    """
    Verify a JWT token.
    
    This endpoint verifies a JWT token and returns its decoded contents.
    """
    logger.info("Token verification requested")
    
    try:
        # Verify token
        token_data = verify_token(verify_request.token)
        
        return JWTVerifyResponse(
            valid=True,
            username=token_data.username,
            permissions=token_data.permissions,
            roles=token_data.roles,
            auth_level=token_data.auth_level,
            identity_provider=token_data.identity_provider
        )
        
    except HTTPException as e:
        return JWTVerifyResponse(
            valid=False,
            username=None,
            permissions=[],
            roles=[],
            auth_level=None,
            identity_provider=None
        )
    except Exception as e:
        logger.error(f"Error verifying token: {e}")
        raise HTTPException(
            status_code=500,
            detail=f"Error verifying token: {str(e)}"
        )


@router.get(
    "/decode",
    response_model=JWTDecodeResponse,
    summary="Decode JWT token",
    description="Decode a JWT token without verification (for debugging).",
    responses={
        200: {"description": "Token decoded successfully"},
        400: {"description": "Invalid token format"},
        500: {"description": "Internal server error"}
    }
)
async def decode_token(
    request: Request,
    token: str
) -> JWTDecodeResponse:
    """
    Decode a JWT token without verification.
    
    This endpoint decodes a JWT token and returns its header and payload.
    WARNING: This does not verify the token signature!
    """
    logger.info("Token decode requested")
    
    try:
        # Decode token without verification
        header = jwt.get_unverified_header(token)
        payload = jwt.get_unverified_claims(token)
        
        return JWTDecodeResponse(
            header=header,
            payload=payload,
            signature="[REDACTED]"
        )
        
    except JWTError as e:
        logger.error(f"Error decoding token: {e}")
        raise HTTPException(
            status_code=400,
            detail=f"Invalid token format: {str(e)}"
        )
    except Exception as e:
        logger.error(f"Error decoding token: {e}")
        raise HTTPException(
            status_code=500,
            detail=f"Error decoding token: {str(e)}"
        )


@router.post(
    "/verify-any-algorithm",
    response_model=JWTVerifyResponse,
    summary="Verify JWT with any algorithm (VULNERABLE)",
    description="Verify a JWT token with any algorithm - VULNERABLE FOR TESTING ONLY!",
    responses={
        200: {"description": "Token verified successfully"},
        401: {"description": "Invalid token"},
        500: {"description": "Internal server error"}
    }
)
async def verify_any_algorithm(
    request: Request,
    verify_request: JWTVerifyRequest
) -> JWTVerifyResponse:
    """
    Verify a JWT token with any algorithm - VULNERABLE!
    
    WARNING: This endpoint is intentionally vulnerable for security testing.
    It accepts tokens with any algorithm, including 'none'.
    """
    logger.warning("Vulnerable token verification requested")
    
    try:
        # VULNERABLE: Decode without proper verification
        payload = jwt.get_unverified_claims(verify_request.token)
        
        return JWTVerifyResponse(
            valid=True,
            username=payload.get("sub"),
            permissions=payload.get("permissions", []),
            roles=payload.get("roles", []),
            auth_level=payload.get("auth_level"),
            identity_provider=payload.get("identity_provider")
        )
        
    except Exception as e:
        logger.error(f"Error in vulnerable verification: {e}")
        return JWTVerifyResponse(
            valid=False,
            username=None,
            permissions=[],
            roles=[],
            auth_level=None,
            identity_provider=None
        )


@router.post(
    "/create-vulnerable",
    response_model=VulnerableJWTResponse,
    summary="Create vulnerable JWT token (VULNERABLE)",
    description="Create a JWT token with weak security - VULNERABLE FOR TESTING ONLY!",
    responses={
        200: {"description": "Vulnerable token created successfully"},
        500: {"description": "Internal server error"}
    }
)
async def create_vulnerable_token(
    request: Request,
    vulnerable_request: VulnerableJWTRequest
) -> VulnerableJWTResponse:
    """
    Create a vulnerable JWT token.
    
    WARNING: This endpoint creates tokens with weak security for testing purposes.
    """
    logger.warning("Vulnerable token creation requested")
    
    try:
        # VULNERABLE: Create token with weak secret and any algorithm
        weak_secret = "weak-secret"  # nosec B105 - Intentionally vulnerable for security testing
        algorithm = vulnerable_request.algorithm or "HS256"
        
        token_data = {
            "sub": vulnerable_request.user,
            "roles": [vulnerable_request.role],
            "iat": datetime.utcnow(),
            "exp": datetime.utcnow() + timedelta(hours=24)
        }
        
        # Create token with weak security
        token = jwt.encode(token_data, weak_secret, algorithm=algorithm)
        
        return VulnerableJWTResponse(
            token=token,
            algorithm=algorithm,
            vulnerable=True
        )
        
    except Exception as e:
        logger.error(f"Error creating vulnerable token: {e}")
        raise HTTPException(
            status_code=500,
            detail=f"Error creating vulnerable token: {str(e)}"
        )
