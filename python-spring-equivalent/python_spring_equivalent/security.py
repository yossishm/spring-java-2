"""
JWT authentication and authorization system.
"""

import logging
from datetime import datetime, timedelta
from typing import Any, Dict, List, Optional, Union

from fastapi import Depends, HTTPException, status
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from jose import JWTError, jwt
from passlib.context import CryptContext
from pydantic import BaseModel

from .config import settings

logger = logging.getLogger(__name__)

# Password hashing
pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")

# JWT Bearer token scheme
security = HTTPBearer()


class TokenData(BaseModel):
    """Token data model."""
    username: Optional[str] = None
    permissions: List[str] = []
    roles: List[str] = []
    auth_level: Optional[str] = None
    identity_provider: Optional[str] = None


class User(BaseModel):
    """User model."""
    username: str
    email: Optional[str] = None
    permissions: List[str] = []
    roles: List[str] = []
    auth_level: str = "AAL1"
    identity_provider: str = "local"
    disabled: bool = False


class Token(BaseModel):
    """Token response model."""
    access_token: str
    token_type: str


def verify_password(plain_password: str, hashed_password: str) -> bool:
    """Verify a password against its hash."""
    return pwd_context.verify(plain_password, hashed_password)


def get_password_hash(password: str) -> str:
    """Hash a password."""
    return pwd_context.hash(password)


def create_access_token(data: Dict[str, Any], expires_delta: Optional[timedelta] = None) -> str:
    """Create a JWT access token."""
    to_encode = data.copy()
    if expires_delta:
        expire = datetime.utcnow() + expires_delta
    else:
        expire = datetime.utcnow() + timedelta(hours=settings.jwt_expiration_hours)
    
    to_encode.update({"exp": expire})
    encoded_jwt = jwt.encode(to_encode, settings.jwt_secret, algorithm=settings.jwt_algorithm)
    return encoded_jwt


def verify_token(token: str) -> TokenData:
    """Verify and decode a JWT token."""
    credentials_exception = HTTPException(
        status_code=status.HTTP_401_UNAUTHORIZED,
        detail="Could not validate credentials",
        headers={"WWW-Authenticate": "Bearer"},
    )
    
    try:
        payload = jwt.decode(token, settings.jwt_secret, algorithms=[settings.jwt_algorithm])
        username: str = payload.get("sub")
        if username is None:
            raise credentials_exception
        
        token_data = TokenData(
            username=username,
            permissions=payload.get("permissions", []),
            roles=payload.get("roles", []),
            auth_level=payload.get("auth_level", "AAL1"),
            identity_provider=payload.get("identity_provider", "local")
        )
        return token_data
    except JWTError:
        raise credentials_exception


async def get_current_user(credentials: HTTPAuthorizationCredentials = Depends(security)) -> User:
    """Get the current authenticated user."""
    token = credentials.credentials
    token_data = verify_token(token)
    
    # In a real application, you would fetch the user from a database
    # For now, we'll create a mock user based on token data
    user = User(
        username=token_data.username,
        permissions=token_data.permissions,
        roles=token_data.roles,
        auth_level=token_data.auth_level or "AAL1",
        identity_provider=token_data.identity_provider or "local"
    )
    
    if user.disabled:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail="Inactive user"
        )
    
    return user


def require_permission(permission: str):
    """Decorator to require a specific permission."""
    def permission_checker(current_user: User = Depends(get_current_user)) -> User:
        if permission not in current_user.permissions and "ADMIN" not in current_user.roles:
            raise HTTPException(
                status_code=status.HTTP_403_FORBIDDEN,
                detail=f"Permission '{permission}' required"
            )
        return current_user
    return permission_checker


def require_role(role: str):
    """Decorator to require a specific role."""
    def role_checker(current_user: User = Depends(get_current_user)) -> User:
        if role not in current_user.roles:
            raise HTTPException(
                status_code=status.HTTP_403_FORBIDDEN,
                detail=f"Role '{role}' required"
            )
        return current_user
    return role_checker


def require_auth_level(required_level: str):
    """Decorator to require a specific authentication level."""
    auth_levels = {"AAL1": 1, "AAL2": 2, "AAL3": 3}
    
    def auth_level_checker(current_user: User = Depends(get_current_user)) -> User:
        user_level = auth_levels.get(current_user.auth_level, 1)
        required_level_num = auth_levels.get(required_level, 1)
        
        if user_level < required_level_num:
            raise HTTPException(
                status_code=status.HTTP_403_FORBIDDEN,
                detail=f"Authentication level '{required_level}' required"
            )
        return current_user
    return auth_level_checker


def require_identity_provider(provider: str):
    """Decorator to require a specific identity provider."""
    def provider_checker(current_user: User = Depends(get_current_user)) -> User:
        if current_user.identity_provider != provider:
            raise HTTPException(
                status_code=status.HTTP_403_FORBIDDEN,
                detail=f"Identity provider '{provider}' required"
            )
        return current_user
    return provider_checker


def require_any_permission(permissions: List[str]):
    """Decorator to require any of the specified permissions."""
    def permission_checker(current_user: User = Depends(get_current_user)) -> User:
        if not any(perm in current_user.permissions for perm in permissions) and "ADMIN" not in current_user.roles:
            raise HTTPException(
                status_code=status.HTTP_403_FORBIDDEN,
                detail=f"One of permissions {permissions} required"
            )
        return current_user
    return permission_checker


def require_all_permissions(permissions: List[str]):
    """Decorator to require all of the specified permissions."""
    def permission_checker(current_user: User = Depends(get_current_user)) -> User:
        if not all(perm in current_user.permissions for perm in permissions) and "ADMIN" not in current_user.roles:
            raise HTTPException(
                status_code=status.HTTP_403_FORBIDDEN,
                detail=f"All permissions {permissions} required"
            )
        return current_user
    return permission_checker


# Mock user database for demonstration
MOCK_USERS = {
    "admin": User(
        username="admin",
        email="admin@example.com",
        permissions=["CACHE_READ", "CACHE_WRITE", "CACHE_DELETE", "CACHE_ADMIN"],
        roles=["ADMIN"],
        auth_level="AAL3",
        identity_provider="enterprise"
    ),
    "user": User(
        username="user",
        email="user@example.com",
        permissions=["CACHE_READ"],
        roles=["USER"],
        auth_level="AAL1",
        identity_provider="local"
    ),
    "manager": User(
        username="manager",
        email="manager@example.com",
        permissions=["CACHE_READ", "CACHE_WRITE"],
        roles=["MANAGER"],
        auth_level="AAL2",
        identity_provider="enterprise"
    ),
    "readonly": User(
        username="readonly",
        email="readonly@example.com",
        permissions=["CACHE_READ"],
        roles=["READONLY"],
        auth_level="AAL1",
        identity_provider="local"
    ),
    "writer": User(
        username="writer",
        email="writer@example.com",
        permissions=["CACHE_READ", "CACHE_WRITE"],
        roles=["WRITER"],
        auth_level="AAL2",
        identity_provider="local"
    )
}


def get_user(username: str) -> Optional[User]:
    """Get a user by username."""
    return MOCK_USERS.get(username)


def authenticate_user(username: str, password: str) -> Optional[User]:
    """Authenticate a user with username and password."""
    user = get_user(username)
    if not user:
        return None
    # In a real application, you would verify the password hash
    # For demo purposes, we'll accept any password
    return user
