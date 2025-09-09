package hello;

import org.springframework.web.bind.annotation.*;
import org.springframework.http.ResponseEntity;
import org.springframework.http.HttpStatus;
import java.util.Map;
import java.util.HashMap;

/**
 * Test Controller for CVE-2025-58056 reproduction
 * This controller provides endpoints to test HTTP request smuggling vulnerability
 */
@RestController
@RequestMapping("/api")
public class TestController {

    /**
     * Test endpoint that processes chunked requests
     * This endpoint can be used to test for HTTP request smuggling
     */
    @PostMapping("/test")
    public ResponseEntity<Map<String, Object>> testChunked(@RequestBody(required = false) String body) {
        Map<String, Object> response = new HashMap<>();
        response.put("status", "success");
        response.put("message", "Request processed");
        response.put("body", body != null ? body : "No body received");
        response.put("timestamp", System.currentTimeMillis());
        
        return ResponseEntity.ok(response);
    }

    /**
     * Health check endpoint for testing
     */
    @GetMapping("/health")
    public ResponseEntity<Map<String, String>> health() {
        Map<String, String> response = new HashMap<>();
        response.put("status", "UP");
        response.put("service", "test-controller");
        
        return ResponseEntity.ok(response);
    }

    /**
     * Sensitive endpoint that should not be accessible via request smuggling
     */
    @GetMapping("/admin/sensitive")
    public ResponseEntity<Map<String, String>> sensitiveData() {
        Map<String, String> response = new HashMap<>();
        response.put("data", "This is sensitive data that should not be accessible via request smuggling");
        response.put("access", "admin-only");
        
        return ResponseEntity.ok(response);
    }

    /**
     * Endpoint to test chunked transfer encoding
     */
    @PostMapping("/chunked")
    public ResponseEntity<Map<String, Object>> testChunkedEncoding(
            @RequestBody(required = false) String body,
            @RequestHeader(value = "Transfer-Encoding", required = false) String transferEncoding) {
        
        Map<String, Object> response = new HashMap<>();
        response.put("status", "chunked-request-processed");
        response.put("body", body != null ? body : "No body received");
        response.put("transferEncoding", transferEncoding);
        response.put("timestamp", System.currentTimeMillis());
        
        return ResponseEntity.ok(response);
    }
}
