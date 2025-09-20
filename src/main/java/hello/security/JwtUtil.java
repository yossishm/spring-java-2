package hello.security;

import io.jsonwebtoken.*;
import io.jsonwebtoken.security.Keys;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import javax.crypto.SecretKey;
import java.nio.charset.StandardCharsets;
import java.util.Date;
import java.util.List;
import java.util.Map;
import java.util.function.Function;

/**
 * JWT utility class for token validation, parsing, and claims extraction.
 * Follows Spring Security best practices for JWT handling.
 */
@Component
public class JwtUtil {

    @Value("${jwt.secret:mySecretKey}")
    private String secret;

    @Value("${jwt.expiration:86400000}") // 24 hours default
    private Long expiration;

    /**
     * Extract username from JWT token
     */
    public String extractUsername(String token) {
        return extractClaim(token, Claims::getSubject);
    }

    /**
     * Extract expiration date from JWT token
     */
    public Date extractExpiration(String token) {
        return extractClaim(token, Claims::getExpiration);
    }

    /**
     * Extract specific claim from JWT token
     */
    public <T> T extractClaim(String token, Function<Claims, T> claimsResolver) {
        final Claims claims = extractAllClaims(token);
        return claimsResolver.apply(claims);
    }

    /**
     * Extract all claims from JWT token
     */
    private Claims extractAllClaims(String token) {
        return Jwts.parser()
                .verifyWith(getSigningKey())
                .build()
                .parseSignedClaims(token)
                .getPayload();
    }

    /**
     * Extract roles from JWT token
     */
    @SuppressWarnings("unchecked")
    public List<String> extractRoles(String token) {
        Claims claims = extractAllClaims(token);
        Object roles = claims.get("roles");
        if (roles instanceof List) {
            return (List<String>) roles;
        }
        return List.of();
    }

    /**
     * Extract permissions from JWT token
     */
    @SuppressWarnings("unchecked")
    public List<String> extractPermissions(String token) {
        Claims claims = extractAllClaims(token);
        Object permissions = claims.get("permissions");
        if (permissions instanceof List) {
            return (List<String>) permissions;
        }
        return List.of();
    }

    /**
     * Extract authentication level from JWT token
     * AAL1: Single-factor authentication
     * AAL2: Multi-factor authentication  
     * AAL3: Hardware-based authentication
     */
    public String extractAuthLevel(String token) {
        Claims claims = extractAllClaims(token);
        Object authLevel = claims.get("auth_level");
        if (authLevel instanceof String) {
            return (String) authLevel;
        }
        return "AAL1"; // Default to lowest level
    }

    /**
     * Extract identity provider from JWT token
     */
    public String extractIdentityProvider(String token) {
        Claims claims = extractAllClaims(token);
        Object idp = claims.get("idp");
        if (idp instanceof String) {
            return (String) idp;
        }
        return "local"; // Default to local provider
    }

    /**
     * Extract custom claims from JWT token
     */
    public Map<String, Object> extractCustomClaims(String token) {
        Claims claims = extractAllClaims(token);
        return claims;
    }

    /**
     * Check if token is expired
     */
    private Boolean isTokenExpired(String token) {
        return extractExpiration(token).before(new Date());
    }

    /**
     * Validate JWT token
     */
    public Boolean validateToken(String token) {
        try {
            Jwts.parser()
                .verifyWith(getSigningKey())
                .build()
                .parseSignedClaims(token);
            return !isTokenExpired(token);
        } catch (JwtException | IllegalArgumentException e) {
            return false;
        }
    }

    /**
     * Validate JWT token with username
     */
    public Boolean validateToken(String token, String username) {
        final String extractedUsername = extractUsername(token);
        return (extractedUsername.equals(username) && !isTokenExpired(token));
    }

    /**
     * Check if user has specific role
     */
    public Boolean hasRole(String token, String role) {
        List<String> roles = extractRoles(token);
        return roles.contains(role);
    }

    /**
     * Check if user has specific permission
     */
    public Boolean hasPermission(String token, String permission) {
        List<String> permissions = extractPermissions(token);
        return permissions.contains(permission);
    }

    /**
     * Check if user has any of the specified roles
     */
    public Boolean hasAnyRole(String token, String... roles) {
        List<String> userRoles = extractRoles(token);
        for (String role : roles) {
            if (userRoles.contains(role)) {
                return true;
            }
        }
        return false;
    }

    /**
     * Check if user has any of the specified permissions
     */
    public Boolean hasAnyPermission(String token, String... permissions) {
        List<String> userPermissions = extractPermissions(token);
        for (String permission : permissions) {
            if (userPermissions.contains(permission)) {
                return true;
            }
        }
        return false;
    }

    /**
     * Get signing key for JWT validation
     */
    private SecretKey getSigningKey() {
        byte[] keyBytes = secret.getBytes(StandardCharsets.UTF_8);
        return Keys.hmacShaKeyFor(keyBytes);
    }

    /**
     * Generate JWT token (for testing purposes)
     */
    public String generateToken(String username, List<String> roles, List<String> permissions) {
        return createToken(username, roles, permissions, "AAL1", "local");
    }

    /**
     * Generate JWT token with authentication level and IDP
     */
    public String generateToken(String username, List<String> roles, List<String> permissions, String authLevel, String idp) {
        return createToken(username, roles, permissions, authLevel, idp);
    }


    /**
     * Create JWT token with claims including auth_level and idp
     */
    private String createToken(String subject, List<String> roles, List<String> permissions, String authLevel, String idp) {
        return Jwts.builder()
                .subject(subject)
                .claim("roles", roles)
                .claim("permissions", permissions)
                .claim("auth_level", authLevel)
                .claim("idp", idp)
                .issuedAt(new Date(System.currentTimeMillis()))
                .expiration(new Date(System.currentTimeMillis() + expiration))
                .signWith(getSigningKey())
                .compact();
    }
}
