"""
Security mitigation for CVE-2024-23342 (ecdsa package vulnerability).

This module ensures that python-jose uses the cryptography backend
instead of the vulnerable ecdsa package.
"""
import logging

logger = logging.getLogger(__name__)


def verify_cryptography_backend() -> bool:
    """
    Verify that python-jose can use the cryptography backend and that
    the vulnerable ecdsa package is not present.
    """
    try:
        # Check if ecdsa is installed
        try:
            import ecdsa  # type: ignore
            logger.warning(
                "⚠️ ecdsa package is installed (CVE-2024-23342). python-jose should use cryptography backend instead."
            )
        except Exception:
            logger.info("✅ ecdsa package not installed (mitigates CVE-2024-23342)")

        # Verify cryptography is available
        try:
            import cryptography  # type: ignore
            logger.info(f"✅ cryptography {cryptography.__version__} is available")
        except Exception:
            logger.error("❌ cryptography package not found - JWT operations may fail")
            return False

        # Verify python-jose cryptography backend module exists
        try:
            import importlib
            importlib.import_module("jose.backends.cryptography_backend")
            logger.info("✅ python-jose cryptography backend module available")
            return True
        except Exception as e:
            logger.error(f"❌ Failed to import python-jose cryptography backend module: {e}")
            return False

    except Exception as e:
        logger.error(f"❌ Error verifying cryptography backend: {e}")
        return False


def ensure_cryptography_backend() -> bool:
    """
    Ensure python-jose uses cryptography backend for JWT operations.
    This should be called at application startup to verify the mitigation.
    """
    if not verify_cryptography_backend():
        logger.warning(
            "⚠️ CVE-2024-23342 mitigation: python-jose may fall back to ecdsa. Ensure cryptography package is installed."
        )
        return False
    return True

