package hello.security;

import jakarta.servlet.ServletOutputStream;
import jakarta.servlet.WriteListener;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.security.core.AuthenticationException;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.nio.charset.StandardCharsets;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

class JwtHandlersTest {

    private static class MockServletOutputStream extends ServletOutputStream {
        private final OutputStream delegate;
        MockServletOutputStream(OutputStream delegate) { this.delegate = delegate; }
        @Override public boolean isReady() { return true; }
        @Override public void setWriteListener(WriteListener writeListener) {
            // Not used in these unit tests; response is fully buffered via ByteArrayOutputStream.
        }
        @Override public void write(int b) throws IOException { delegate.write(b); }
    }

    @Test
    @DisplayName("JwtAuthenticationEntryPoint writes 401 JSON body")
    void entryPoint_commence_writesJson() throws Exception {
        JwtAuthenticationEntryPoint entryPoint = new JwtAuthenticationEntryPoint();
        HttpServletRequest request = mock(HttpServletRequest.class);
        when(request.getServletPath()).thenReturn("/api/test");

        HttpServletResponse response = mock(HttpServletResponse.class);
        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        when(response.getOutputStream()).thenReturn(new MockServletOutputStream(baos));

        entryPoint.commence(request, response, new AuthenticationException("bad token") {});

        String body = baos.toString(StandardCharsets.UTF_8);
        assertThat(body).contains("\"status\":401").contains("\"error\":\"Unauthorized\"").contains("\"path\":\"/api/test\"");
    }

    @Test
    @DisplayName("JwtAccessDeniedHandler writes 403 JSON body")
    void accessDenied_handle_writesJson() throws Exception {
        JwtAccessDeniedHandler handler = new JwtAccessDeniedHandler();
        HttpServletRequest request = mock(HttpServletRequest.class);
        when(request.getServletPath()).thenReturn("/api/admin");

        HttpServletResponse response = mock(HttpServletResponse.class);
        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        when(response.getOutputStream()).thenReturn(new MockServletOutputStream(baos));

        handler.handle(request, response, new AccessDeniedException("nope"));

        String body = baos.toString(StandardCharsets.UTF_8);
        assertThat(body).contains("\"status\":403").contains("\"error\":\"Forbidden\"").contains("\"path\":\"/api/admin\"");
    }
}


