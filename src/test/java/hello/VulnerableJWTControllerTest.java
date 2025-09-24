package hello;

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
class VulnerableJWTControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Test
    @DisplayName("POST /api/jwt/create returns token")
    void createToken_returnsToken() throws Exception {
        mockMvc.perform(post("/api/jwt/create")
                .contentType(MediaType.APPLICATION_JSON)
                .content("{\"user\":\"admin\"}"))
            .andExpect(status().isOk());
    }

    @Test
    @DisplayName("POST /api/jwt/verify returns valid true")
    void verifyToken_returnsValidTrue() throws Exception {
        mockMvc.perform(post("/api/jwt/verify")
                .contentType(MediaType.APPLICATION_JSON)
                .content("{\"token\":\"x.y.z\"}"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.valid").value(true));
    }

    @Test
    @DisplayName("GET /api/jwt/decode returns decoded parts")
    void decodeToken_returnsDecoded() throws Exception {
        String token = "aGVhZGVy.cGF5bG9hZA.sign"; // base64 'header' and 'payload'
        mockMvc.perform(get("/api/jwt/decode").param("token", token))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.decoded").value(true));
    }

    @Test
    @DisplayName("POST /api/jwt/verify-any-algorithm returns ok")
    void verifyAnyAlgorithm_returnsOk() throws Exception {
        mockMvc.perform(post("/api/jwt/verify-any-algorithm")
                .contentType(MediaType.APPLICATION_JSON)
                .content("{\"token\":\"x.y.z\",\"algorithm\":\"none\"}"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.valid").value(true));
    }
}


