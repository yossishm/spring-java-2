package hello.security;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.springframework.http.ResponseEntity;
import org.springframework.web.context.request.WebRequest;

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
}
