package hello.controller;

import hello.security.JwtUtil;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Controller for generating JWT tokens for testing purposes.
 * In production, this would typically be handled by an authentication service.
 */
@RestController
@RequestMapping("/api/v1/auth")
public class TokenController {

    @Autowired
    private JwtUtil jwtUtil;

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
            permissions = List.of("CACHE_READ");
        }

        String token = jwtUtil.generateToken(username, roles, permissions);

        Map<String, Object> response = new HashMap<>();
        response.put("token", token);
        response.put("username", username);
        response.put("roles", roles);
        response.put("permissions", permissions);
        response.put("expiresIn", "24 hours");
        response.put("timestamp", System.currentTimeMillis());

        return ResponseEntity.ok(response);
    }

    /**
     * Generate predefined token types for testing
     */
    @GetMapping("/token/{type}")
    public ResponseEntity<Map<String, Object>> generatePredefinedToken(@PathVariable String type) {
        Map<String, Object> response = new HashMap<>();
        String token;
        String username;
        List<String> roles;
        List<String> permissions;

        switch (type.toLowerCase()) {
            case "admin":
                username = "admin";
                roles = List.of("ADMIN", "USER");
                permissions = List.of("CACHE_READ", "CACHE_WRITE", "CACHE_DELETE", "CACHE_ADMIN");
                break;
            case "user":
                username = "user";
                roles = List.of("USER");
                permissions = List.of("CACHE_READ");
                break;
            case "cache-admin":
                username = "cache-admin";
                roles = List.of("USER");
                permissions = List.of("CACHE_READ", "CACHE_WRITE", "CACHE_DELETE", "CACHE_ADMIN");
                break;
            case "cache-writer":
                username = "cache-writer";
                roles = List.of("USER");
                permissions = List.of("CACHE_READ", "CACHE_WRITE");
                break;
            case "cache-reader":
                username = "cache-reader";
                roles = List.of("USER");
                permissions = List.of("CACHE_READ");
                break;
            default:
                return ResponseEntity.badRequest().body(Map.of("error", "Invalid token type"));
        }

        token = jwtUtil.generateToken(username, roles, permissions);

        response.put("token", token);
        response.put("username", username);
        response.put("roles", roles);
        response.put("permissions", permissions);
        response.put("type", type);
        response.put("expiresIn", "24 hours");
        response.put("timestamp", System.currentTimeMillis());

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
            response.put("username", jwtUtil.extractUsername(token));
            response.put("roles", jwtUtil.extractRoles(token));
            response.put("permissions", jwtUtil.extractPermissions(token));
        }
        
        response.put("timestamp", System.currentTimeMillis());
        
        return ResponseEntity.ok(response);
    }
}


