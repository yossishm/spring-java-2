# Security Configuration Guide

## JWT Secret Configuration

### Production Setup
1. Set the `JWT_SECRET` environment variable with a strong, randomly generated secret:
   ```bash
   export JWT_SECRET="your-super-secure-jwt-secret-key-here-minimum-32-characters"
   ```

2. Or use a secrets management service like Azure Key Vault, AWS Secrets Manager, or HashiCorp Vault.

### Development Setup
For development, you can use the default configuration in `appsettings.json`, but ensure you:
- Never commit real secrets to version control
- Use different secrets for different environments
- Rotate secrets regularly

## Security Best Practices

### 1. Secret Management
- ✅ Use environment variables for secrets
- ✅ Use strong, randomly generated secrets (minimum 32 characters)
- ✅ Never hardcode secrets in source code
- ✅ Use different secrets for different environments
- ✅ Rotate secrets regularly

### 2. JWT Security
- ✅ Use strong signing keys
- ✅ Set appropriate expiration times
- ✅ Validate issuer and audience in production
- ✅ Use HTTPS in production

### 3. Vulnerable Endpoints
The following endpoints are intentionally vulnerable for demonstration purposes:
- `/api/jwt/create` - Uses weak secret and simple base64 encoding
- `/api/jwt/verify` - No proper validation
- `/api/jwt/decode` - Decodes without verification
- `/api/jwt/verify-any-algorithm` - Accepts any algorithm

**⚠️ WARNING: These endpoints should NEVER be used in production!**

## Environment Variables

| Variable | Description | Required | Example |
|----------|-------------|----------|---------|
| `JWT_SECRET` | JWT signing secret | Yes | `your-super-secure-secret-key` |
| `JWT_EXPIRATION_HOURS` | Token expiration time | No | `24` |

## Docker Security

When using Docker, ensure you:
- Use multi-stage builds to reduce attack surface
- Don't copy sensitive files to the container
- Use non-root users
- Keep base images updated
- Scan images for vulnerabilities
