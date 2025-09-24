package hello;

import io.micrometer.core.instrument.simple.SimpleMeterRegistry;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Import;
import org.springframework.test.web.servlet.MockMvc;

import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.content;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@SpringBootTest
@AutoConfigureMockMvc(addFilters = false)
@Import(MetricsControllerTest.TestConfig.class)
@org.junit.jupiter.api.Disabled("Replaced by unit-level test to avoid mapping issues")
class MetricsControllerTest {

    @Configuration
    static class TestConfig {
        @Bean
        public SimpleMeterRegistry meterRegistry() {
            return new SimpleMeterRegistry();
        }
    }

    @Autowired
    private MockMvc mockMvc;

    @Test
    @DisplayName("GET /api/metrics/test returns 200 and contains text")
    void testMetrics_returnsOk() throws Exception {
        mockMvc.perform(get("/api/metrics/test"))
            .andExpect(status().isOk())
            .andExpect(content().string(org.hamcrest.Matchers.containsString("Metrics test")));
    }

    @Test
    @DisplayName("GET /api/metrics/counter increments and returns 200")
    void incrementCounter_returnsOk() throws Exception {
        mockMvc.perform(get("/api/metrics/counter").param("value", "2"))
            .andExpect(status().isOk())
            .andExpect(content().string(org.hamcrest.Matchers.containsString("Counter incremented by 2")));
    }

    @Test
    @DisplayName("GET /api/metrics/slow returns 200 and contains text")
    void slowEndpoint_returnsOk() throws Exception {
        mockMvc.perform(get("/api/metrics/slow"))
            .andExpect(status().isOk())
            .andExpect(content().string(org.hamcrest.Matchers.containsString("operation")));
    }
}


