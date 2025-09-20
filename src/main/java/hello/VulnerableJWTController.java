package hello;

import org.springframework.web.bind.annotation.*;
import org.springframework.http.ResponseEntity;

import java.util.HashMap;
import java.util.Map;
import java.util.Base64;
import java.nio.charset.StandardCharsets;

@RestController
@RequestMapping("/api/jwt")
public class VulnerableJWTController {

    // Vulnerable: Using weak secret key
    private static final String WEAK_SECRET = "mySecretKey123";

    @PostMapping("/create")
    public ResponseEntity<String> createToken(@RequestBody Map<String, Object> payload) {
        try {
            // Vulnerable: Simple base64 encoding (not real JWT)
            String header = Base64.getEncoder().encodeToString("{\"alg\":\"HS256\",\"typ\":\"JWT\"}".getBytes(StandardCharsets.UTF_8));
            String payloadStr = Base64.getEncoder().encodeToString(payload.toString().getBytes(StandardCharsets.UTF_8));
            String signature = Base64.getEncoder().encodeToString(WEAK_SECRET.getBytes(StandardCharsets.UTF_8));
            
            String token = header + "." + payloadStr + "." + signature;
            return ResponseEntity.ok(token);
        } catch (Exception e) {
            return ResponseEntity.badRequest().body("Error creating token: " + e.getMessage());
        }
    }

    @PostMapping("/verify")
    public ResponseEntity<Map<String, Object>> verifyToken(@RequestBody Map<String, String> request) {
        try {
            String token = request.get("token");
            
            // Vulnerable: No proper validation
            Map<String, Object> response = new HashMap<>();
            response.put("valid", true);
            response.put("token", token);
            response.put("message", "Token accepted without proper validation");
            
            return ResponseEntity.ok(response);
        } catch (Exception e) {
            Map<String, Object> response = new HashMap<>();
            response.put("valid", false);
            response.put("error", e.getMessage());
            
            return ResponseEntity.ok(response);
        }
    }

    @GetMapping("/decode")
    public ResponseEntity<Map<String, Object>> decodeToken(@RequestParam String token) {
        try {
            // Vulnerable: Decoding without verification
            String[] parts = token.split("\\.");
            Map<String, Object> response = new HashMap<>();
            response.put("decoded", true);
            response.put("header", parts.length > 0 ? new String(Base64.getDecoder().decode(parts[0]), StandardCharsets.UTF_8) : "");
            response.put("payload", parts.length > 1 ? new String(Base64.getDecoder().decode(parts[1]), StandardCharsets.UTF_8) : "");
            response.put("signature", parts.length > 2 ? parts[2] : "");
            
            return ResponseEntity.ok(response);
        } catch (Exception e) {
            Map<String, Object> response = new HashMap<>();
            response.put("decoded", false);
            response.put("error", e.getMessage());
            
            return ResponseEntity.ok(response);
        }
    }

    @PostMapping("/verify-any-algorithm")
    public ResponseEntity<Map<String, Object>> verifyAnyAlgorithm(@RequestBody Map<String, String> request) {
        try {
            String token = request.get("token");
            String algorithm = request.get("algorithm"); // Vulnerable: user-controlled algorithm
            
            // Vulnerable: Accepts any algorithm without validation
            Map<String, Object> response = new HashMap<>();
            response.put("valid", true);
            response.put("algorithm", algorithm);
            response.put("token", token);
            response.put("message", "Algorithm accepted without validation: " + algorithm);
            
            return ResponseEntity.ok(response);
        } catch (Exception e) {
            Map<String, Object> response = new HashMap<>();
            response.put("valid", false);
            response.put("error", e.getMessage());
            
            return ResponseEntity.ok(response);
        }
    }
}
