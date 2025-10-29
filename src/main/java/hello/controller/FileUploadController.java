package hello.controller;

import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;
import org.springframework.web.multipart.MaxUploadSizeExceededException;
import org.springframework.http.ResponseEntity;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
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
 * File upload controller that demonstrates CVE-2025-61795 vulnerability.
 * This controller intentionally has a small file size limit to trigger
 * the vulnerability when multipart uploads exceed the limit.
 */
@RestController
@RequestMapping("/api/v1/files")
@Tag(name = "File Upload", description = "File upload operations that demonstrate CVE-2025-61795")
public class FileUploadController {

    private static final Logger logger = LoggerFactory.getLogger(FileUploadController.class);
    
    // Small file size limit to trigger CVE-2025-61795
    private static final long MAX_FILE_SIZE = 1024; // 1KB limit to easily trigger the vulnerability
    
    /**
     * Upload endpoint that demonstrates CVE-2025-61795.
     * When file size exceeds the limit, Tomcat creates temporary files
     * that are not properly cleaned up due to the vulnerability.
     */
    @PostMapping(value = "/upload", consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    @Operation(
        summary = "Upload file (CVE-2025-61795 vulnerable endpoint)",
        description = "Uploads a file with a small size limit to demonstrate CVE-2025-61795. " +
                     "Files larger than 1KB will trigger the vulnerability where temporary files " +
                     "are not properly cleaned up."
    )
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "File uploaded successfully"),
        @ApiResponse(responseCode = "413", description = "File too large - triggers CVE-2025-61795"),
        @ApiResponse(responseCode = "400", description = "Bad request")
    })
    public ResponseEntity<Map<String, Object>> uploadFile(
        @Parameter(description = "File to upload", required = true)
        @RequestParam("file") MultipartFile file) {
        
        logger.info("File upload attempt: {} (size: {} bytes)", 
                   file.getOriginalFilename(), file.getSize());
        
        try {
            // Process the file (in a real app, you'd save it somewhere)
            String content = new String(file.getBytes());
            logger.info("File processed successfully: {} characters", content.length());
            
            return ResponseEntity.ok(Map.of(
                "message", "File uploaded successfully",
                "filename", file.getOriginalFilename(),
                "size", file.getSize(),
                "contentLength", content.length()
            ));
            
        } catch (IOException e) {
            logger.error("Error processing file", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(Map.of("error", "Failed to process file: " + e.getMessage()));
        }
    }
    
    /**
     * Exception handler for multipart upload size exceeded.
     * This is where CVE-2025-61795 vulnerability occurs - temporary files
     * are created but not immediately cleaned up.
     */
    @ExceptionHandler(MaxUploadSizeExceededException.class)
    public ResponseEntity<Map<String, Object>> handleMaxUploadSizeExceeded(MaxUploadSizeExceededException ex) {
        logger.warn("File upload size exceeded - this triggers CVE-2025-61795: {}", ex.getMessage());
        
        // This is where the vulnerability occurs - temp files are created but not cleaned up
        return ResponseEntity.status(HttpStatus.PAYLOAD_TOO_LARGE)
            .body(Map.of(
                "error", "File too large",
                "maxSize", "1KB",
                "vulnerability", "CVE-2025-61795 triggered - temporary files not cleaned up",
                "message", "This demonstrates the vulnerability where temp files accumulate"
            ));
    }

    /**
     * Endpoint to check system resources and temporary file accumulation.
     * This helps monitor the impact of CVE-2025-61795.
     */
    @GetMapping("/system-info")
    @Operation(
        summary = "Get system information",
        description = "Returns system information to monitor the impact of CVE-2025-61795"
    )
    public ResponseEntity<Map<String, Object>> getSystemInfo() {
        try {
            Runtime runtime = Runtime.getRuntime();
            long totalMemory = runtime.totalMemory();
            long freeMemory = runtime.freeMemory();
            long usedMemory = totalMemory - freeMemory;
            long maxMemory = runtime.maxMemory();
            
            // Check for temporary files in system temp directory
            Path tempDir = Paths.get(System.getProperty("java.io.tmpdir"));
            long tempFileCount = 0;
            long tempFileSize = 0;
            
            try {
                tempFileCount = Files.list(tempDir)
                    .filter(path -> path.getFileName().toString().startsWith("tomcat"))
                    .count();
                
                tempFileSize = Files.list(tempDir)
                    .filter(path -> path.getFileName().toString().startsWith("tomcat"))
                    .mapToLong(path -> {
                        try {
                            return Files.size(path);
                        } catch (IOException e) {
                            return 0;
                        }
                    })
                    .sum();
            } catch (IOException e) {
                logger.warn("Could not check temp directory", e);
            }
            
            return ResponseEntity.ok(Map.of(
                "memory", Map.of(
                    "total", totalMemory,
                    "used", usedMemory,
                    "free", freeMemory,
                    "max", maxMemory,
                    "usagePercent", (double) usedMemory / maxMemory * 100
                ),
                "tempFiles", Map.of(
                    "count", tempFileCount,
                    "totalSize", tempFileSize,
                    "directory", tempDir.toString()
                ),
                "vulnerability", "CVE-2025-61795 - check temp file accumulation"
            ));
            
        } catch (Exception e) {
            logger.error("Error getting system info", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(Map.of("error", "Failed to get system info: " + e.getMessage()));
        }
    }
}
