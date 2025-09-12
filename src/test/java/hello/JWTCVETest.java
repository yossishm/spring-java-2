package hello;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.context.SpringBootTest.WebEnvironment;
import org.springframework.boot.test.web.client.TestRestTemplate;
import org.springframework.boot.test.web.server.LocalServerPort;
import org.springframework.context.annotation.Import;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;

import java.util.HashMap;
import java.util.Map;

import static org.assertj.core.api.Assertions.assertThat;

@SpringBootTest(webEnvironment = WebEnvironment.RANDOM_PORT, properties = "spring.profiles.active=test")
@Import(TestSecurityConfig.class)
public class JWTCVETest {

    @LocalServerPort
    private int port;

    @Autowired
    private TestRestTemplate restTemplate;

    @Test
    public void testVulnerableJWTTokenCreation() {
        // Test creating a JWT token with vulnerable configuration
        Map<String, Object> payload = new HashMap<>();
        payload.put("user", "admin");
        payload.put("role", "admin");
        payload.put("exp", System.currentTimeMillis() + 3600000);

        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        HttpEntity<Map<String, Object>> request = new HttpEntity<>(payload, headers);

        ResponseEntity<String> response = restTemplate.postForEntity(
            "http://localhost:" + port + "/api/jwt/create", 
            request, 
            String.class
        );

        assertThat(response.getStatusCode().is2xxSuccessful()).isTrue();
        assertThat(response.getBody()).isNotNull();
        assertThat(response.getBody()).contains(".");
    }

    @Test
    public void testVulnerableJWTTokenVerification() {
        // Test JWT token verification with vulnerable configuration
        Map<String, String> request = new HashMap<>();
        request.put("token", "eyJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoiYWRtaW4iLCJyb2xlIjoiYWRtaW4ifQ.invalid_signature");

        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        HttpEntity<Map<String, String>> entity = new HttpEntity<>(request, headers);

        ResponseEntity<Map> response = restTemplate.postForEntity(
            "http://localhost:" + port + "/api/jwt/verify", 
            entity, 
            Map.class
        );

        assertThat(response.getStatusCode().is2xxSuccessful()).isTrue();
        assertThat(response.getBody()).isNotNull();
    }

    @Test
    public void testVulnerableJWTDecode() {
        // Test JWT token decoding (vulnerable to information disclosure)
        ResponseEntity<Map> response = restTemplate.getForEntity(
            "http://localhost:" + port + "/api/jwt/decode?token=eyJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoiYWRtaW4iLCJyb2xlIjoiYWRtaW4ifQ.invalid_signature", 
            Map.class
        );

        assertThat(response.getStatusCode().is2xxSuccessful()).isTrue();
        assertThat(response.getBody()).isNotNull();
    }

    @Test
    public void testVulnerableAlgorithmInjection() {
        // Test algorithm injection vulnerability
        Map<String, String> request = new HashMap<>();
        request.put("token", "eyJhbGciOiJub25lIn0.eyJ1c2VyIjoiYWRtaW4ifQ.");
        request.put("algorithm", "NONE"); // Vulnerable: none algorithm

        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        HttpEntity<Map<String, String>> entity = new HttpEntity<>(request, headers);

        ResponseEntity<Map> response = restTemplate.postForEntity(
            "http://localhost:" + port + "/api/jwt/verify-any-algorithm", 
            entity, 
            Map.class
        );

        assertThat(response.getStatusCode().is2xxSuccessful()).isTrue();
        assertThat(response.getBody()).isNotNull();
    }
}

