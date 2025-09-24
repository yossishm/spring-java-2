package hello.security;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.springframework.test.util.ReflectionTestUtils;

import java.util.List;

import static org.assertj.core.api.Assertions.assertThat;

class JwtUtilTest {

    @Test
    @DisplayName("JwtUtil generates and validates token; extracts claims")
    void generateAndValidateToken_extractClaims() {
        JwtUtil jwtUtil = new JwtUtil();
        ReflectionTestUtils.setField(jwtUtil, "secret", "mySecretKeymySecretKeymySecretKey123");
        ReflectionTestUtils.setField(jwtUtil, "expiration", 3600_000L);

        String token = jwtUtil.generateToken("alice", List.of("ADMIN"), List.of("CACHE_READ", "CACHE_WRITE"));

        assertThat(jwtUtil.validateToken(token)).isTrue();
        assertThat(jwtUtil.extractUsername(token)).isEqualTo("alice");
        assertThat(jwtUtil.extractRoles(token)).contains("ADMIN");
        assertThat(jwtUtil.extractPermissions(token)).contains("CACHE_READ", "CACHE_WRITE");
        assertThat(jwtUtil.hasRole(token, "ADMIN")).isTrue();
        assertThat(jwtUtil.hasAnyRole(token, "USER", "ADMIN")).isTrue();
        assertThat(jwtUtil.hasPermission(token, "CACHE_READ")).isTrue();
        assertThat(jwtUtil.hasAnyPermission(token, "X", "CACHE_WRITE")).isTrue();

        // Negative branches
        assertThat(jwtUtil.hasRole(token, "NOPE")).isFalse();
        assertThat(jwtUtil.hasPermission(token, "NOPE")).isFalse();
        assertThat(jwtUtil.hasAnyRole(token, "ZZZ")).isFalse();
        assertThat(jwtUtil.hasAnyPermission(token, "YYY")).isFalse();
    }

    @Test
    @DisplayName("JwtUtil validateToken with username (non-expired) and expired token handling")
    void validateTokenWithUsername_andExpired() throws Exception {
        JwtUtil jwtUtil = new JwtUtil();
        org.springframework.test.util.ReflectionTestUtils.setField(jwtUtil, "secret", "mySecretKeymySecretKeymySecretKey123");
        // Use a safe expiration for username-validation case
        org.springframework.test.util.ReflectionTestUtils.setField(jwtUtil, "expiration", 60_000L);

        String token = jwtUtil.generateToken("carol", java.util.List.of("USER"), java.util.List.of());
        assertThat(jwtUtil.validateToken(token, "carol")).isTrue();

        // Now create an immediately expired token and validate with the boolean API that handles expiry
        org.springframework.test.util.ReflectionTestUtils.setField(jwtUtil, "expiration", 0L);
        String expired = jwtUtil.generateToken("dave", java.util.List.of("USER"), java.util.List.of());
        assertThat(jwtUtil.validateToken(expired)).isFalse();
    }
}


