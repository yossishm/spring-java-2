package hello.security;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.springframework.http.ResponseEntity;
import org.springframework.web.context.request.WebRequest;
import org.springframework.security.authentication.BadCredentialsException;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.security.core.AuthenticationException;
import io.jsonwebtoken.JwtException;

import java.util.Map;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

class GlobalExceptionHandlerTest {

    private final GlobalExceptionHandler handler = new GlobalExceptionHandler();

    private WebRequest mockRequest() {
        WebRequest request = mock(WebRequest.class);
        when(request.getDescription(false)).thenReturn("uri=/dummy");
        return request;
    }

    @Test
    @DisplayName("handleGenericException returns 500 with message")
    void handleGenericException() {
        WebRequest request = mockRequest();
        ResponseEntity<Map<String, Object>> response = handler.handleGenericException(new Exception("boom"), request);
        assertThat(response.getStatusCode().value()).isEqualTo(500);
        assertThat(response.getBody()).containsKey("message");
    }

    @Test
    @DisplayName("handleIllegalArgumentException returns 400")
    void handleIllegalArgument() {
        WebRequest request = mockRequest();
        ResponseEntity<Map<String, Object>> response = handler.handleIllegalArgumentException(new IllegalArgumentException("bad"), request);
        assertThat(response.getStatusCode().value()).isEqualTo(400);
    }

    @Test
    @DisplayName("handleAccessDeniedException returns 403")
    void handleAccessDenied() {
        WebRequest request = mockRequest();
        ResponseEntity<Map<String, Object>> response = handler.handleAccessDeniedException(new AccessDeniedException("no"), request);
        assertThat(response.getStatusCode().value()).isEqualTo(403);
    }

    @Test
    @DisplayName("handleAuthenticationException returns 401")
    void handleAuthException() {
        WebRequest request = mockRequest();
        AuthenticationException ex = new AuthenticationException("auth") {};
        ResponseEntity<Map<String, Object>> response = handler.handleAuthenticationException(ex, request);
        assertThat(response.getStatusCode().value()).isEqualTo(401);
    }

    @Test
    @DisplayName("handleBadCredentialsException returns 401")
    void handleBadCreds() {
        WebRequest request = mockRequest();
        ResponseEntity<Map<String, Object>> response = handler.handleBadCredentialsException(new BadCredentialsException("bad"), request);
        assertThat(response.getStatusCode().value()).isEqualTo(401);
    }

    @Test
    @DisplayName("handleJwtException returns 401")
    void handleJwt() {
        WebRequest request = mockRequest();
        ResponseEntity<Map<String, Object>> response = handler.handleJwtException(new JwtException("jwt"), request);
        assertThat(response.getStatusCode().value()).isEqualTo(401);
    }
}
