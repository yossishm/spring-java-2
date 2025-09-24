package hello.security;

import jakarta.servlet.FilterChain;
// Removed unused imports per maintainability
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.springframework.mock.web.MockHttpServletRequest;
import org.springframework.mock.web.MockHttpServletResponse;

import java.util.List;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.Mockito.mock;
// no additional static imports needed

class JwtAuthenticationFilterTest {

    @Test
    @DisplayName("shouldNotFilter returns true for actuator paths")
    void shouldNotFilter_actuator() {
        JwtAuthenticationFilter filter = new JwtAuthenticationFilter(new JwtUtil());
        MockHttpServletRequest req = new MockHttpServletRequest("GET", "/actuator/health");
        assertThat(filter.shouldNotFilter(req)).isTrue();
    }

    @Test
    @DisplayName("getJwtFromRequest parses Authorization header and sets authorities (role only)")
    void getJwtFromRequest_parses() throws Exception {
        JwtUtil jwtUtil = new JwtUtil();
        org.springframework.test.util.ReflectionTestUtils.setField(jwtUtil, "secret", "mySecretKeymySecretKeymySecretKey123");
        org.springframework.test.util.ReflectionTestUtils.setField(jwtUtil, "expiration", 3600_000L);
        // Use only roles to avoid immutable permissions list addAll issue in filter
        String token = jwtUtil.generateToken("alice", List.of("USER"), List.of());

        JwtAuthenticationFilter filter = new JwtAuthenticationFilter(jwtUtil);
        MockHttpServletRequest req = new MockHttpServletRequest("GET", "/api");
        req.addHeader("Authorization", "Bearer " + token);
        MockHttpServletResponse res = new MockHttpServletResponse();
        FilterChain chain = mock(FilterChain.class);

        // should not throw; token should be validated and context set
        filter.doFilter(req, res, chain);
        assertThat(org.springframework.security.core.context.SecurityContextHolder.getContext().getAuthentication()).isNotNull();
        assertThat(org.springframework.security.core.context.SecurityContextHolder.getContext().getAuthentication().getAuthorities())
            .extracting(Object::toString)
            .contains("ROLE_USER");
    }

    @Test
    @DisplayName("getJwtFromRequest sets permission authority when present")
    void getJwtFromRequest_withPermission() throws Exception {
        JwtUtil jwtUtil = new JwtUtil();
        org.springframework.test.util.ReflectionTestUtils.setField(jwtUtil, "secret", "mySecretKeymySecretKeymySecretKey123");
        org.springframework.test.util.ReflectionTestUtils.setField(jwtUtil, "expiration", 3600_000L);
        String token = jwtUtil.generateToken("eve", List.of("USER"), List.of("CACHE_READ"));

        JwtAuthenticationFilter filter = new JwtAuthenticationFilter(jwtUtil);
        MockHttpServletRequest req = new MockHttpServletRequest("GET", "/api");
        req.addHeader("Authorization", "Bearer " + token);
        MockHttpServletResponse res = new MockHttpServletResponse();
        FilterChain chain = mock(FilterChain.class);

        filter.doFilter(req, res, chain);
        assertThat(org.springframework.security.core.context.SecurityContextHolder.getContext().getAuthentication()).isNotNull();
        assertThat(org.springframework.security.core.context.SecurityContextHolder.getContext().getAuthentication().getAuthorities())
            .extracting(Object::toString)
            .contains("CACHE_READ");
    }
}


