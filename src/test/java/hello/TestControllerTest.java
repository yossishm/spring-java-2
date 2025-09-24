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
class TestControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Test
    @DisplayName("POST /api/test returns ok with body")
    void testChunked_returnsOk() throws Exception {
        mockMvc.perform(post("/api/test")
                .contentType(MediaType.TEXT_PLAIN)
                .content("hello"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.status").value("success"))
            .andExpect(jsonPath("$.body").value("hello"));
    }

    @Test
    @DisplayName("GET /api/health returns UP")
    void health_returnsUp() throws Exception {
        mockMvc.perform(get("/api/health"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.status").value("UP"));
    }

    @Test
    @DisplayName("GET /api/admin/sensitive returns data")
    void sensitive_returnsData() throws Exception {
        mockMvc.perform(get("/api/admin/sensitive"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.data").exists());
    }

    @Test
    @DisplayName("POST /api/chunked returns ok")
    void testChunkedEncoding_returnsOk() throws Exception {
        mockMvc.perform(post("/api/chunked")
                .contentType(MediaType.TEXT_PLAIN)
                .content("world"))
            .andExpect(status().isOk());
    }
}


