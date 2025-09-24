package hello;

import io.micrometer.core.instrument.simple.SimpleMeterRegistry;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import static org.assertj.core.api.Assertions.assertThat;

class MetricsControllerUnitTest {

    @Test
    @DisplayName("MetricsController endpoints work without web layer")
    void metricsController_directCalls() {
        SimpleMeterRegistry registry = new SimpleMeterRegistry();
        MetricsController controller = new MetricsController(registry);

        String test = controller.testMetrics();
        assertThat(test).contains("Metrics test");

        String counter = controller.incrementCounter(2);
        assertThat(counter).contains("Counter incremented by 2");

        String slow = controller.slowEndpoint();
        assertThat(slow).contains("operation");
    }
}


