package hello.controller;

import hello.security.RequirePermission;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import java.util.HashMap;
import java.util.Map;

/**
 * Test controller to demonstrate JWT authorization functionality.
 * Shows different levels of authorization and permission checking.
 */
@RestController
@RequestMapping("/api/v1/test")
public class AuthTestController {

    /**
     * Public endpoint - no authentication required
     */
    @GetMapping("/public")
    public ResponseEntity<Map<String, Object>> publicEndpoint() {
        Map<String, Object> response = new HashMap<>();
        response.put("message", "This is a public endpoint");
        response.put("timestamp", System.currentTimeMillis());
        return ResponseEntity.ok(response);
    }

    /**
     * Protected endpoint - requires authentication
     */
    @GetMapping("/protected")
    public ResponseEntity<Map<String, Object>> protectedEndpoint() {
        Map<String, Object> response = new HashMap<>();
        response.put("message", "This is a protected endpoint");
        response.put("timestamp", System.currentTimeMillis());
        return ResponseEntity.ok(response);
    }

    /**
     * Admin endpoint - requires ADMIN role
     */
    @GetMapping("/admin")
    @PreAuthorize("hasRole('ADMIN')")
    @RequirePermission(roles = {"ADMIN"})
    public ResponseEntity<Map<String, Object>> adminEndpoint() {
        Map<String, Object> response = new HashMap<>();
        response.put("message", "This is an admin-only endpoint");
        response.put("timestamp", System.currentTimeMillis());
        return ResponseEntity.ok(response);
    }

    /**
     * Cache read endpoint - requires CACHE_READ permission
     */
    @GetMapping("/cache/read")
    @PreAuthorize("hasAuthority('CACHE_READ')")
    @RequirePermission(value = {"CACHE_READ"})
    public ResponseEntity<Map<String, Object>> cacheReadEndpoint() {
        Map<String, Object> response = new HashMap<>();
        response.put("message", "Cache read operation successful");
        response.put("timestamp", System.currentTimeMillis());
        return ResponseEntity.ok(response);
    }

    /**
     * Cache write endpoint - requires CACHE_WRITE permission
     */
    @PostMapping("/cache/write")
    @PreAuthorize("hasAuthority('CACHE_WRITE')")
    @RequirePermission(value = {"CACHE_WRITE"})
    public ResponseEntity<Map<String, Object>> cacheWriteEndpoint(@RequestBody Map<String, Object> data) {
        Map<String, Object> response = new HashMap<>();
        response.put("message", "Cache write operation successful");
        response.put("data", data);
        response.put("timestamp", System.currentTimeMillis());
        return ResponseEntity.ok(response);
    }

    /**
     * Cache admin endpoint - requires CACHE_ADMIN permission
     */
    @DeleteMapping("/cache/admin")
    @PreAuthorize("hasAuthority('CACHE_ADMIN')")
    @RequirePermission(value = {"CACHE_ADMIN"})
    public ResponseEntity<Map<String, Object>> cacheAdminEndpoint() {
        Map<String, Object> response = new HashMap<>();
        response.put("message", "Cache admin operation successful");
        response.put("timestamp", System.currentTimeMillis());
        return ResponseEntity.ok(response);
    }

    /**
     * Multiple permissions endpoint - requires ANY of the specified permissions
     */
    @GetMapping("/multi-permission")
    @PreAuthorize("hasAnyAuthority('CACHE_READ', 'CACHE_WRITE', 'CACHE_ADMIN')")
    @RequirePermission(value = {"CACHE_READ", "CACHE_WRITE", "CACHE_ADMIN"})
    public ResponseEntity<Map<String, Object>> multiPermissionEndpoint() {
        Map<String, Object> response = new HashMap<>();
        response.put("message", "Multi-permission endpoint accessed successfully");
        response.put("timestamp", System.currentTimeMillis());
        return ResponseEntity.ok(response);
    }

    /**
     * All permissions endpoint - requires ALL specified permissions
     */
    @GetMapping("/all-permissions")
    @RequirePermission(value = {"CACHE_READ", "CACHE_WRITE"}, requireAll = true)
    public ResponseEntity<Map<String, Object>> allPermissionsEndpoint() {
        Map<String, Object> response = new HashMap<>();
        response.put("message", "All permissions endpoint accessed successfully");
        response.put("timestamp", System.currentTimeMillis());
        return ResponseEntity.ok(response);
    }
}


