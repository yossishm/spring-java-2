package hello.security;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.security.authentication.BadCredentialsException;
import org.springframework.security.core.AuthenticationException;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.context.request.WebRequest;
import org.springframework.web.multipart.MaxUploadSizeExceededException;

import java.time.LocalDateTime;
import java.util.HashMap;
import java.util.Map;

/**
 * Global exception handler for security-related exceptions.
 * Provides consistent error responses for authentication and authorization failures.
 */
@ControllerAdvice
public class GlobalExceptionHandler {

    // Constants for duplicated literals
    private static final String TIMESTAMP_KEY = "timestamp";
    private static final String STATUS_KEY = "status";
    private static final String ERROR_KEY = "error";
    private static final String MESSAGE_KEY = "message";
    private static final String PATH_KEY = "path";
    private static final String UNAUTHORIZED = "Unauthorized";

    /**
     * Handle access denied exceptions (403 Forbidden)
     */
    @ExceptionHandler(AccessDeniedException.class)
    public ResponseEntity<Map<String, Object>> handleAccessDeniedException(
            AccessDeniedException ex, WebRequest request) {
        
        Map<String, Object> body = new HashMap<>();
        body.put(TIMESTAMP_KEY, LocalDateTime.now());
        body.put(STATUS_KEY, HttpStatus.FORBIDDEN.value());
        body.put(ERROR_KEY, "Forbidden");
        body.put(MESSAGE_KEY, ex.getMessage());
        body.put(PATH_KEY, request.getDescription(false).replace("uri=", ""));
        
        return new ResponseEntity<>(body, HttpStatus.FORBIDDEN);
    }

    /**
     * Handle authentication exceptions (401 Unauthorized)
     */
    @ExceptionHandler(AuthenticationException.class)
    public ResponseEntity<Map<String, Object>> handleAuthenticationException(
            AuthenticationException ex, WebRequest request) {
        
        Map<String, Object> body = new HashMap<>();
        body.put(TIMESTAMP_KEY, LocalDateTime.now());
        body.put(STATUS_KEY, HttpStatus.UNAUTHORIZED.value());
        body.put(ERROR_KEY, UNAUTHORIZED);
        body.put(MESSAGE_KEY, ex.getMessage());
        body.put(PATH_KEY, request.getDescription(false).replace("uri=", ""));
        
        return new ResponseEntity<>(body, HttpStatus.UNAUTHORIZED);
    }

    /**
     * Handle bad credentials exceptions (401 Unauthorized)
     */
    @ExceptionHandler(BadCredentialsException.class)
    public ResponseEntity<Map<String, Object>> handleBadCredentialsException(
            BadCredentialsException ex, WebRequest request) {
        
        Map<String, Object> body = new HashMap<>();
        body.put(TIMESTAMP_KEY, LocalDateTime.now());
        body.put(STATUS_KEY, HttpStatus.UNAUTHORIZED.value());
        body.put(ERROR_KEY, UNAUTHORIZED);
        body.put(MESSAGE_KEY, "Invalid credentials");
        body.put(PATH_KEY, request.getDescription(false).replace("uri=", ""));
        
        return new ResponseEntity<>(body, HttpStatus.UNAUTHORIZED);
    }

    /**
     * Handle JWT-related exceptions
     */
    @ExceptionHandler(io.jsonwebtoken.JwtException.class)
    public ResponseEntity<Map<String, Object>> handleJwtException(
            io.jsonwebtoken.JwtException ex, WebRequest request) {
        
        Map<String, Object> body = new HashMap<>();
        body.put(TIMESTAMP_KEY, LocalDateTime.now());
        body.put(STATUS_KEY, HttpStatus.UNAUTHORIZED.value());
        body.put(ERROR_KEY, UNAUTHORIZED);
        body.put(MESSAGE_KEY, "Invalid JWT token: " + ex.getMessage());
        body.put(PATH_KEY, request.getDescription(false).replace("uri=", ""));
        
        return new ResponseEntity<>(body, HttpStatus.UNAUTHORIZED);
    }

    /**
     * Handle illegal argument exceptions (400 Bad Request)
     */
    @ExceptionHandler(IllegalArgumentException.class)
    public ResponseEntity<Map<String, Object>> handleIllegalArgumentException(
            IllegalArgumentException ex, WebRequest request) {
        
        Map<String, Object> body = new HashMap<>();
        body.put(TIMESTAMP_KEY, LocalDateTime.now());
        body.put(STATUS_KEY, HttpStatus.BAD_REQUEST.value());
        body.put(ERROR_KEY, "Bad Request");
        body.put(MESSAGE_KEY, ex.getMessage());
        body.put(PATH_KEY, request.getDescription(false).replace("uri=", ""));
        
        return new ResponseEntity<>(body, HttpStatus.BAD_REQUEST);
    }

    /**
     * Handle multipart upload size exceeded exceptions (413 Payload Too Large)
     * This is where CVE-2025-61795 vulnerability occurs - temporary files
     * are created but not immediately cleaned up.
     */
    @ExceptionHandler(MaxUploadSizeExceededException.class)
    public ResponseEntity<Map<String, Object>> handleMaxUploadSizeExceededException(
            MaxUploadSizeExceededException ex, WebRequest request) {
        
        Map<String, Object> body = new HashMap<>();
        body.put(TIMESTAMP_KEY, LocalDateTime.now());
        body.put(STATUS_KEY, HttpStatus.PAYLOAD_TOO_LARGE.value());
        body.put(ERROR_KEY, "File too large");
        body.put(MESSAGE_KEY, "CVE-2025-61795 triggered - temporary files not cleaned up");
        body.put(PATH_KEY, request.getDescription(false).replace("uri=", ""));
        body.put("vulnerability", "CVE-2025-61795");
        body.put("maxSize", "1KB");
        
        return new ResponseEntity<>(body, HttpStatus.PAYLOAD_TOO_LARGE);
    }

    /**
     * Handle general exceptions (500 Internal Server Error)
     */
    @ExceptionHandler(Exception.class)
    public ResponseEntity<Map<String, Object>> handleGenericException(
            Exception ex, WebRequest request) {
        
        Map<String, Object> body = new HashMap<>();
        body.put(TIMESTAMP_KEY, LocalDateTime.now());
        body.put(STATUS_KEY, HttpStatus.INTERNAL_SERVER_ERROR.value());
        body.put(ERROR_KEY, "Internal Server Error");
        body.put(MESSAGE_KEY, "An unexpected error occurred");
        body.put(PATH_KEY, request.getDescription(false).replace("uri=", ""));
        
        return new ResponseEntity<>(body, HttpStatus.INTERNAL_SERVER_ERROR);
    }
}


