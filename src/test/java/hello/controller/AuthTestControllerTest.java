package hello.controller;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.web.servlet.MockMvc;

import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.delete;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.jsonPath;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@SpringBootTest
@AutoConfigureMockMvc(addFilters = false)
class AuthTestControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Test
    @DisplayName("GET /api/v1/test/public returns 200 and message")
    void publicEndpoint_returnsOk() throws Exception {
        mockMvc.perform(get("/api/v1/test/public"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.message").value("This is a public endpoint"));
    }

    @Test
    @DisplayName("GET /api/v1/test/protected returns 200 and message")
    void protectedEndpoint_returnsOk() throws Exception {
        mockMvc.perform(get("/api/v1/test/protected"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.message").value("This is a protected endpoint"));
    }

    @Test
    @DisplayName("GET /api/v1/test/admin returns 200 and message")
    void adminEndpoint_returnsOk() throws Exception {
        mockMvc.perform(get("/api/v1/test/admin"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.message").value("This is an admin-only endpoint"));
    }

    @Test
    @DisplayName("GET /api/v1/test/cache/read returns 200 and message")
    void cacheRead_returnsOk() throws Exception {
        mockMvc.perform(get("/api/v1/test/cache/read"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.message").value("Cache read operation successful"));
    }

    @Test
    @DisplayName("POST /api/v1/test/cache/write returns 200 and message")
    void cacheWrite_returnsOk() throws Exception {
        mockMvc.perform(post("/api/v1/test/cache/write").contentType("application/json").content("{\"k\":\"v\"}"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.message").value("Cache write operation successful"))
            .andExpect(jsonPath("$.data.k").value("v"));
    }

    @Test
    @DisplayName("DELETE /api/v1/test/cache/admin returns 200 and message")
    void cacheAdmin_returnsOk() throws Exception {
        mockMvc.perform(delete("/api/v1/test/cache/admin"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.message").value("Cache admin operation successful"));
    }

    @Test
    @DisplayName("GET /api/v1/test/multi-permission returns 200")
    void multiPermission_returnsOk() throws Exception {
        mockMvc.perform(get("/api/v1/test/multi-permission"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.message").value("Multi-permission endpoint accessed successfully"));
    }

    @Test
    @DisplayName("GET /api/v1/test/all-permissions returns 200")
    void allPermissions_returnsOk() throws Exception {
        mockMvc.perform(get("/api/v1/test/all-permissions"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.message").value("All permissions endpoint accessed successfully"));
    }
}


