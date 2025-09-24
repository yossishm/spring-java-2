package hello.controller;

import hello.security.JwtUtil;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;

import java.util.HashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;

/**
 * Controller for generating JWT tokens for testing purposes.
 * In production, this would typically be handled by an authentication service.
 */
@RestController
@RequestMapping("/api/v1/auth")
@Tag(name = "Authentication", description = "JWT token generation and validation endpoints")
public class TokenController {

    private final JwtUtil jwtUtil;
    
    // Constants for duplicated literals
    private static final String CACHE_READ = "CACHE_READ";
    private static final String USERNAME_KEY = "username";
    private static final String ROLES_KEY = "roles";
    private static final String PERMISSIONS_KEY = "permissions";
    private static final String TIMESTAMP_KEY = "timestamp";
    private static final String CACHE_DELETE = "CACHE_DELETE";
    private static final String CACHE_ADMIN = "CACHE_ADMIN";
    private static final String CACHE_WRITE = "CACHE_WRITE";
    private static final String LOCAL = "local";

    public TokenController(JwtUtil jwtUtil) {
        this.jwtUtil = jwtUtil;
    }

    /**
     * Generate a JWT token with specified roles and permissions
     */
    @PostMapping("/token")
    public ResponseEntity<Map<String, Object>> generateToken(
            @RequestParam String username,
            @RequestParam(required = false) List<String> roles,
            @RequestParam(required = false) List<String> permissions) {
        
        // Default roles and permissions if not provided
        if (roles == null || roles.isEmpty()) {
            roles = List.of("USER");
        }
        if (permissions == null || permissions.isEmpty()) {
            permissions = List.of(CACHE_READ);
        }

        String token = jwtUtil.generateToken(username, roles, permissions);

        Map<String, Object> response = new HashMap<>();
        response.put("token", token);
        response.put(USERNAME_KEY, username);
        response.put(ROLES_KEY, roles);
        response.put(PERMISSIONS_KEY, permissions);
        response.put("expiresIn", "24 hours");
        response.put(TIMESTAMP_KEY, System.currentTimeMillis());

        return ResponseEntity.ok(response);
    }

    /**
     * Generate predefined token types for testing
     */
    @GetMapping("/token/{type}")
    @Operation(
        summary = "Generate predefined JWT token",
        description = """
            Generates JWT tokens with predefined roles and permissions for testing.
            
            **Available token types:**
            - `admin` - Full admin access (AAL3, enterprise-ldap, ADMIN role, all permissions)
            - `user` - Basic user access (AAL1, local, USER role, CACHE_READ permission)
            - `cache-admin` - Cache administration (AAL2, azure-ad, USER role, all cache permissions)
            - `cache-writer` - Cache read/write access (AAL2, okta, USER role, CACHE_READ + CACHE_WRITE)
            - `cache-reader` - Cache read-only access (AAL1, local, USER role, CACHE_READ only)
            - `aal1-user` - AAL1 authentication level (local IDP)
            - `aal2-user` - AAL2 authentication level (azure-ad IDP)
            - `aal3-user` - AAL3 authentication level (enterprise-ldap IDP)
            
            **Authentication Assurance Levels (AAL):**
            - `AAL1` - Single-factor authentication (password only)
            - `AAL2` - Multi-factor authentication (password + MFA)
            - `AAL3` - Hardware-based authentication (FIDO2, smart cards)
            """
    )
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Token generated successfully"),
        @ApiResponse(responseCode = "400", description = "Invalid token type")
    })
    public ResponseEntity<Map<String, Object>> generatePredefinedToken(
        @Parameter(description = "Token type", required = true, example = "admin")
        @PathVariable String type) {
        Map<String, Object> response = new HashMap<>();
        String token;
        String username;
        List<String> roles;
        List<String> permissions;

        String authLevel;
        String idp;
        
        switch (type.toLowerCase(Locale.ROOT)) {
            case "admin":
                username = "admin";
                roles = List.of("ADMIN", "USER");
                permissions = List.of(CACHE_READ, CACHE_WRITE, CACHE_DELETE, CACHE_ADMIN);
                authLevel = "AAL3"; // High security for admin
                idp = "enterprise-ldap";
                break;
            case "user":
                username = "user";
                roles = List.of("USER");
                permissions = List.of(CACHE_READ);
                authLevel = "AAL1"; // Basic auth for regular user
                idp = LOCAL;
                break;
            case "cache-admin":
                username = "cache-admin";
                roles = List.of("USER");
                permissions = List.of(CACHE_READ, CACHE_WRITE, CACHE_DELETE, CACHE_ADMIN);
                authLevel = "AAL2"; // MFA for cache admin
                idp = "azure-ad";
                break;
            case "cache-writer":
                username = "cache-writer";
                roles = List.of("USER");
                permissions = List.of(CACHE_READ, CACHE_WRITE);
                authLevel = "AAL2"; // MFA for write operations
                idp = "okta";
                break;
            case "cache-reader":
                username = "cache-reader";
                roles = List.of("USER");
                permissions = List.of(CACHE_READ);
                authLevel = "AAL1"; // Basic auth for read-only
                idp = LOCAL;
                break;
            case "aal1-user":
                username = "aal1-user";
                roles = List.of("USER");
                permissions = List.of(CACHE_READ);
                authLevel = "AAL1";
                idp = LOCAL;
                break;
            case "aal2-user":
                username = "aal2-user";
                roles = List.of("USER");
                permissions = List.of(CACHE_READ, CACHE_WRITE);
                authLevel = "AAL2";
                idp = "azure-ad";
                break;
            case "aal3-user":
                username = "aal3-user";
                roles = List.of("ADMIN");
                permissions = List.of(CACHE_READ, CACHE_WRITE, CACHE_DELETE, CACHE_ADMIN);
                authLevel = "AAL3";
                idp = "enterprise-ldap";
                break;
            default:
                return ResponseEntity.badRequest().body(Map.of("error", "Invalid token type"));
        }

        token = jwtUtil.generateToken(username, roles, permissions, authLevel, idp);

        response.put("token", token);
        response.put(USERNAME_KEY, username);
        response.put(ROLES_KEY, roles);
        response.put(PERMISSIONS_KEY, permissions);
        response.put("auth_level", authLevel);
        response.put("idp", idp);
        response.put("type", type);
        response.put("expiresIn", "24 hours");
        response.put(TIMESTAMP_KEY, System.currentTimeMillis());

        return ResponseEntity.ok(response);
    }

    /**
     * Validate a JWT token
     */
    @PostMapping("/validate")
    public ResponseEntity<Map<String, Object>> validateToken(@RequestParam String token) {
        Map<String, Object> response = new HashMap<>();
        
        boolean isValid = jwtUtil.validateToken(token);
        response.put("valid", isValid);
        
        if (isValid) {
            response.put(USERNAME_KEY, jwtUtil.extractUsername(token));
            response.put(ROLES_KEY, jwtUtil.extractRoles(token));
            response.put(PERMISSIONS_KEY, jwtUtil.extractPermissions(token));
            response.put("auth_level", jwtUtil.extractAuthLevel(token));
            response.put("idp", jwtUtil.extractIdentityProvider(token));
        }
        
        response.put(TIMESTAMP_KEY, System.currentTimeMillis());
        
        return ResponseEntity.ok(response);
    }
}


