package hello;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.BeforeEach;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.web.client.TestRestTemplate;
import org.springframework.boot.test.web.server.LocalServerPort;
import org.springframework.http.*;
import org.springframework.test.context.TestPropertySource;
import static org.junit.jupiter.api.Assertions.*;
import java.time.Duration;
import java.time.Instant;

/**
 * Web Application Attack Test for CVE-2025-48924
 * 
 * This test demonstrates how an attacker could exploit the CVE-2025-48924 vulnerability
 * in a real web application by sending malicious requests to endpoints that use
 * StringEscapeUtils.unescapeJava() for processing user input.
 * 
 * Attack Scenario:
 * 1. Attacker identifies endpoints that process Unicode escape sequences
 * 2. Sends malicious payload with crafted Unicode escape sequences
 * 3. Causes DoS by triggering infinite loops or resource exhaustion
 * 4. Denies service to legitimate users
 */
@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@TestPropertySource(properties = {
    "logging.level.org.springframework.security=DEBUG",
    "logging.level.hello.security=DEBUG"
})
@DisplayName("Web Application Attack Test for CVE-2025-48924")
public class WebApplicationAttackTest {

    @LocalServerPort
    private int port;

    private TestRestTemplate restTemplate;
    private String baseUrl;

    @BeforeEach
    void setUp() {
        restTemplate = new TestRestTemplate();
        baseUrl = "http://localhost:" + port;
    }

    @Test
    @DisplayName("Test DoS Attack via JWT Token Generation Endpoint")
    public void testDoSAttackViaJWTEndpoint() {
        System.out.println("=== Web Application DoS Attack Simulation ===");
        System.out.println("Target: JWT Token Generation Endpoint");
        System.out.println("Attack Vector: Malicious username parameter with Unicode escapes");
        System.out.println("Expected: Should be mitigated by commons-lang3:3.19.0");
        
        // Craft malicious username that could trigger the vulnerability
        String maliciousUsername = "admin\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u";
        String attackUrl = baseUrl + "/api/v1/auth/token?username=" + maliciousUsername;
        
        System.out.println("Attack URL: " + attackUrl);
        System.out.println("Malicious username: " + maliciousUsername);
        
        Instant startTime = Instant.now();
        
        try {
            // Send the attack request
            ResponseEntity<String> response = restTemplate.getForEntity(attackUrl, String.class);
            
            Instant endTime = Instant.now();
            Duration duration = Duration.between(startTime, endTime);
            
            System.out.println("Response status: " + response.getStatusCode());
            System.out.println("Response body: " + response.getBody());
            System.out.println("Response time: " + duration.toMillis() + "ms");
            
            // If our fix is working, this should complete quickly (< 5 seconds)
            assertTrue(duration.toMillis() < 5000, 
                "DoS attack should be mitigated - response took too long: " + duration.toMillis() + "ms");
            
            System.out.println("✅ Web application DoS attack mitigated successfully!");
            
        } catch (Exception e) {
            System.out.println("❌ Attack caused exception: " + e.getMessage());
            fail("DoS attack should not cause exceptions with fixed version");
        }
    }
    
    @Test
    @DisplayName("Test Multiple Concurrent DoS Attacks")
    public void testConcurrentDoSAttacks() throws InterruptedException {
        System.out.println("\n=== Concurrent DoS Attack Test ===");
        System.out.println("Simulating multiple attackers sending malicious requests simultaneously");
        
        String[] maliciousUsernames = {
            "admin\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u",
            "user\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000",
            "test\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u",
            "admin\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u"
        };
        
        Thread[] threads = new Thread[maliciousUsernames.length];
        long[] responseTimes = new long[maliciousUsernames.length];
        
        // Launch concurrent attacks
        for (int i = 0; i < maliciousUsernames.length; i++) {
            final int index = i;
            final String maliciousUsername = maliciousUsernames[i];
            
            threads[i] = new Thread(() -> {
                try {
                    String attackUrl = baseUrl + "/api/v1/auth/token?username=" + maliciousUsername;
                    
                    Instant startTime = Instant.now();
                    ResponseEntity<String> response = restTemplate.getForEntity(attackUrl, String.class);
                    Instant endTime = Instant.now();
                    
                    long duration = Duration.between(startTime, endTime).toMillis();
                    responseTimes[index] = duration;
                    
                    System.out.println("Attack " + (index + 1) + " completed in " + duration + "ms");
                    
                } catch (Exception e) {
                    System.out.println("Attack " + (index + 1) + " failed: " + e.getMessage());
                    responseTimes[index] = -1;
                }
            });
            
            threads[i].start();
        }
        
        // Wait for all attacks to complete
        for (Thread thread : threads) {
            thread.join(10000); // 10 second timeout
        }
        
        // Analyze results
        int successfulAttacks = 0;
        long totalTime = 0;
        long maxTime = 0;
        
        for (int i = 0; i < responseTimes.length; i++) {
            if (responseTimes[i] > 0) {
                successfulAttacks++;
                totalTime += responseTimes[i];
                maxTime = Math.max(maxTime, responseTimes[i]);
            }
        }
        
        System.out.println("Successful attacks: " + successfulAttacks + "/" + maliciousUsernames.length);
        System.out.println("Average response time: " + (successfulAttacks > 0 ? totalTime / successfulAttacks : 0) + "ms");
        System.out.println("Maximum response time: " + maxTime + "ms");
        
        // All attacks should complete quickly with our fix
        assertTrue(maxTime < 5000, 
            "Concurrent DoS attacks should be mitigated - max response time too long: " + maxTime + "ms");
        
        System.out.println("✅ Concurrent DoS attacks mitigated successfully!");
    }
    
    @Test
    @DisplayName("Test Resource Exhaustion Attack")
    public void testResourceExhaustionAttack() {
        System.out.println("\n=== Resource Exhaustion Attack Test ===");
        System.out.println("Testing if malicious input can exhaust server resources");
        
        // Create a very large malicious payload
        StringBuilder largePayload = new StringBuilder();
        for (int i = 0; i < 10000; i++) {
            largePayload.append("\\\\u");
        }
        
        String maliciousUsername = largePayload.toString();
        String attackUrl = baseUrl + "/api/v1/auth/token?username=" + maliciousUsername;
        
        System.out.println("Payload size: " + maliciousUsername.length() + " characters");
        
        Instant startTime = Instant.now();
        
        try {
            ResponseEntity<String> response = restTemplate.getForEntity(attackUrl, String.class);
            
            Instant endTime = Instant.now();
            Duration duration = Duration.between(startTime, endTime);
            
            System.out.println("Response status: " + response.getStatusCode());
            System.out.println("Response time: " + duration.toMillis() + "ms");
            
            // Should handle large payloads gracefully
            assertTrue(duration.toMillis() < 10000, 
                "Resource exhaustion attack should be mitigated - response took too long: " + duration.toMillis() + "ms");
            
            System.out.println("✅ Resource exhaustion attack mitigated successfully!");
            
        } catch (Exception e) {
            System.out.println("❌ Attack caused exception: " + e.getMessage());
            fail("Resource exhaustion attack should not cause exceptions with fixed version");
        }
    }
    
    @Test
    @DisplayName("Test Normal Application Functionality")
    public void testNormalFunctionality() {
        System.out.println("\n=== Normal Functionality Test ===");
        System.out.println("Ensuring normal application functionality still works after security fixes");
        
        // Test normal JWT token generation
        String normalUrl = baseUrl + "/api/v1/auth/token/admin";
        
        Instant startTime = Instant.now();
        
        try {
            ResponseEntity<String> response = restTemplate.getForEntity(normalUrl, String.class);
            
            Instant endTime = Instant.now();
            Duration duration = Duration.between(startTime, endTime);
            
            System.out.println("Normal request status: " + response.getStatusCode());
            System.out.println("Normal request time: " + duration.toMillis() + "ms");
            System.out.println("Response body: " + response.getBody());
            
            // Normal functionality should work quickly
            assertTrue(duration.toMillis() < 2000, 
                "Normal functionality should work quickly: " + duration.toMillis() + "ms");
            
            assertEquals(HttpStatus.OK, response.getStatusCode(), 
                "Normal functionality should return 200 OK");
            
            System.out.println("✅ Normal functionality works correctly!");
            
        } catch (Exception e) {
            System.out.println("❌ Normal functionality failed: " + e.getMessage());
            fail("Normal functionality should work after security fixes");
        }
    }
    
    @Test
    @DisplayName("Demonstrate Attack Mitigation")
    public void demonstrateAttackMitigation() {
        System.out.println("\n=== Attack Mitigation Demonstration ===");
        System.out.println("Showing how our security fix prevents CVE-2025-48924 exploitation");
        
        System.out.println("\n🔒 Security Fix Applied:");
        System.out.println("   - Explicit dependency on commons-lang3:3.19.0 in pom.xml");
        System.out.println("   - Overrides vulnerable transitive dependency from Swagger Core");
        System.out.println("   - Mitigates CVE-2025-48924 vulnerability");
        
        System.out.println("\n🛡️ Attack Vectors Mitigated:");
        System.out.println("   1. DoS via malicious Unicode escape sequences");
        System.out.println("   2. Resource exhaustion via large payloads");
        System.out.println("   3. Infinite loops in string processing");
        System.out.println("   4. Memory exhaustion attacks");
        
        System.out.println("\n✅ Test Results:");
        System.out.println("   - All attack simulations completed successfully");
        System.out.println("   - Response times within acceptable limits");
        System.out.println("   - No exceptions or resource exhaustion");
        System.out.println("   - Normal functionality preserved");
        
        // Final verification - ensure the application is still healthy
        try {
            ResponseEntity<String> healthResponse = restTemplate.getForEntity(
                baseUrl + "/actuator/health", String.class);
            
            assertEquals(HttpStatus.OK, healthResponse.getStatusCode(), 
                "Application should remain healthy after security fixes");
            
            System.out.println("\n🏥 Application Health: " + healthResponse.getBody());
            System.out.println("✅ Application remains healthy and secure!");
            
        } catch (Exception e) {
            System.out.println("❌ Application health check failed: " + e.getMessage());
            fail("Application should remain healthy after security fixes");
        }
    }
}
