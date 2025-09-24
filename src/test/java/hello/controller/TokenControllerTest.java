package hello.controller;

import hello.security.JwtUtil;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.http.MediaType;
import org.springframework.test.web.servlet.MockMvc;

import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.jsonPath;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@SpringBootTest
@AutoConfigureMockMvc(addFilters = false)
class TokenControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Autowired
    private JwtUtil jwtUtil;

    @Test
    @DisplayName("POST /api/v1/auth/token generates token")
    void generateToken_returnsToken() throws Exception {
        mockMvc.perform(post("/api/v1/auth/token")
                .param("username", "alice"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.token").exists())
            .andExpect(jsonPath("$.username").value("alice"));
    }

    @Test
    @DisplayName("GET /api/v1/auth/token/admin returns admin token and claims")
    void generatePredefinedToken_admin() throws Exception {
        mockMvc.perform(get("/api/v1/auth/token/admin"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.type").value("admin"))
            .andExpect(jsonPath("$.auth_level").value("AAL3"))
            .andExpect(jsonPath("$.idp").value("enterprise-ldap"));
    }

    @Test
    @DisplayName("POST /api/v1/auth/validate returns valid and details")
    void validateToken_returnsDetails() throws Exception {
        String token = jwtUtil.generateToken("bob", java.util.List.of("USER"), java.util.List.of("CACHE_READ"));
        mockMvc.perform(post("/api/v1/auth/validate")
                .contentType(MediaType.APPLICATION_FORM_URLENCODED)
                .param("token", token))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.valid").value(true))
            .andExpect(jsonPath("$.username").value("bob"));
    }

    @Test
    @DisplayName("GET /api/v1/auth/token/{type} invalid type returns 400")
    void generatePredefinedToken_invalidType() throws Exception {
        mockMvc.perform(get("/api/v1/auth/token/does-not-exist"))
            .andExpect(status().isBadRequest())
            .andExpect(jsonPath("$.error").exists());
    }
}
