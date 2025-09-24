package hello;

import io.micrometer.core.instrument.simple.SimpleMeterRegistry;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import static org.assertj.core.api.Assertions.assertThat;

class PrometheusControllerUnitTest {

    @Test
    @DisplayName("PrometheusController returns metrics text")
    void prometheusMetrics_directCall() {
        SimpleMeterRegistry registry = new SimpleMeterRegistry();
        PrometheusController controller = new PrometheusController(registry);
        String metrics = controller.prometheusMetrics();
        assertThat(metrics).contains("prometheus_requests_total");
    }
}


