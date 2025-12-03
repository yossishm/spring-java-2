package hello.controller;

import org.springframework.web.bind.annotation.*;
import org.springframework.http.ResponseEntity;
import org.springframework.http.HttpStatus;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Map;

/**
 * Vulnerable controller that demonstrates CVE-2025-24813 and CVE-2025-55752.
 * 
 * This controller intentionally has vulnerable path handling to demonstrate:
 * - CVE-2025-24813: Path equivalence vulnerability in Tomcat
 * - CVE-2025-55752: Relative path traversal vulnerability in Tomcat
 * 
 * WARNING: This controller is intentionally vulnerable for testing purposes only.
 * Do not use this pattern in production code.
 */
@RestController
@RequestMapping("/api/v1/files")
@Tag(name = "Vulnerable Path Operations", description = "Intentionally vulnerable endpoints for CVE testing")
public class VulnerablePathController {

    private static final Logger logger = LoggerFactory.getLogger(VulnerablePathController.class);
    private static final String BASE_DIR = "/tmp/vulnerable-files";

    /**
     * Vulnerable file access endpoint that demonstrates CVE-2025-24813 and CVE-2025-55752.
     * This endpoint does not properly sanitize paths, allowing path traversal attacks.
     */
    @GetMapping("/read/**")
    @Operation(
        summary = "Read file (VULNERABLE - CVE-2025-24813, CVE-2025-55752)",
        description = "Intentionally vulnerable endpoint that demonstrates path equivalence " +
                     "and path traversal vulnerabilities. Do not use in production."
    )
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "File read (may be vulnerable)"),
        @ApiResponse(responseCode = "404", description = "File not found"),
        @ApiResponse(responseCode = "403", description = "Access denied")
    })
    public ResponseEntity<Map<String, Object>> readFile(
        @RequestParam(value = "path", required = false) String filePath) {
        
        logger.warn("Vulnerable file read endpoint accessed - CVE-2025-24813/CVE-2025-55752");
        
        if (filePath == null || filePath.isEmpty()) {
            return ResponseEntity.badRequest()
                .body(Map.of("error", "Path parameter required"));
        }
        
        try {
            // VULNERABLE: Direct path usage without proper sanitization
            // This allows path traversal attacks (CVE-2025-55752)
            Path requestedPath = Paths.get(BASE_DIR, filePath).normalize();
            
            // VULNERABLE: Path equivalence not properly checked (CVE-2025-24813)
            // The normalize() call may not be sufficient in vulnerable Tomcat versions
            
            if (!Files.exists(requestedPath)) {
                return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body(Map.of(
                        "error", "File not found",
                        "requestedPath", requestedPath.toString(),
                        "vulnerability", "CVE-2025-24813, CVE-2025-55752"
                    ));
            }
            
            if (!Files.isRegularFile(requestedPath)) {
                return ResponseEntity.status(HttpStatus.FORBIDDEN)
                    .body(Map.of(
                        "error", "Path is not a regular file",
                        "vulnerability", "CVE-2025-24813, CVE-2025-55752"
                    ));
            }
            
            // Read file content (vulnerable to path traversal)
            String content = Files.readString(requestedPath);
            
            logger.warn("File read completed - potential vulnerability exploited: {}", requestedPath);
            
            return ResponseEntity.ok(Map.of(
                "content", content,
                "path", requestedPath.toString(),
                "vulnerability", "CVE-2025-24813, CVE-2025-55752 - path traversal possible",
                "warning", "This endpoint is intentionally vulnerable for testing"
            ));
            
        } catch (IOException e) {
            logger.error("Error reading file", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(Map.of(
                    "error", "Failed to read file: " + e.getMessage(),
                    "vulnerability", "CVE-2025-24813, CVE-2025-55752"
                ));
        }
    }

    /**
     * Vulnerable file write endpoint (if PUT is enabled).
     * This demonstrates RCE potential with CVE-2025-55752.
     */
    @PutMapping("/write/**")
    @Operation(
        summary = "Write file (VULNERABLE - CVE-2025-55752 RCE potential)",
        description = "Intentionally vulnerable endpoint that demonstrates RCE potential " +
                     "with path traversal. Do not use in production."
    )
    @ApiResponses(value = {
        @ApiResponse(responseCode = "201", description = "File written (may be vulnerable)"),
        @ApiResponse(responseCode = "403", description = "Access denied")
    })
    public ResponseEntity<Map<String, Object>> writeFile(
        @RequestParam(value = "path", required = false) String filePath,
        @RequestBody(required = false) String content) {
        
        logger.warn("Vulnerable file write endpoint accessed - CVE-2025-55752 RCE potential");
        
        if (filePath == null || filePath.isEmpty()) {
            return ResponseEntity.badRequest()
                .body(Map.of("error", "Path parameter required"));
        }
        
        if (content == null) {
            content = "";
        }
        
        try {
            // VULNERABLE: Direct path usage without proper sanitization
            Path requestedPath = Paths.get(BASE_DIR, filePath).normalize();
            
            // Create parent directories if needed (vulnerable)
            Files.createDirectories(requestedPath.getParent());
            
            // Write file (vulnerable to path traversal)
            Files.writeString(requestedPath, content);
            
            logger.warn("File written - potential RCE vulnerability exploited: {}", requestedPath);
            
            return ResponseEntity.status(HttpStatus.CREATED)
                .body(Map.of(
                    "message", "File written",
                    "path", requestedPath.toString(),
                    "vulnerability", "CVE-2025-55752 - RCE potential if PUT enabled",
                    "warning", "This endpoint is intentionally vulnerable for testing"
                ));
            
        } catch (IOException e) {
            logger.error("Error writing file", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(Map.of(
                    "error", "Failed to write file: " + e.getMessage(),
                    "vulnerability", "CVE-2025-55752"
                ));
        }
    }

    /**
     * Endpoint to demonstrate path equivalence attacks (CVE-2025-24813).
     */
    @GetMapping("/equiv/**")
    @Operation(
        summary = "Path equivalence test (VULNERABLE - CVE-2025-24813)",
        description = "Intentionally vulnerable endpoint to demonstrate path equivalence attacks"
    )
    public ResponseEntity<Map<String, Object>> pathEquivalence(
        @RequestParam(value = "path", required = false) String filePath) {
        
        logger.warn("Path equivalence endpoint accessed - CVE-2025-24813");
        
        if (filePath == null || filePath.isEmpty()) {
            return ResponseEntity.badRequest()
                .body(Map.of("error", "Path parameter required"));
        }
        
        // VULNERABLE: Path equivalence not properly handled
        // Various path equivalence patterns can bypass security constraints
        Path requestedPath = Paths.get(BASE_DIR, filePath);
        
        return ResponseEntity.ok(Map.of(
            "originalPath", filePath,
            "resolvedPath", requestedPath.toString(),
            "normalizedPath", requestedPath.normalize().toString(),
            "vulnerability", "CVE-2025-24813 - path equivalence vulnerability",
            "warning", "This endpoint is intentionally vulnerable for testing",
            "note", "In vulnerable Tomcat versions, path equivalence can bypass security"
        ));
    }
}


