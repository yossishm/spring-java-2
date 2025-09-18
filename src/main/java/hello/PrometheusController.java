package hello;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;
import io.micrometer.core.instrument.MeterRegistry;
import io.micrometer.core.instrument.Counter;
import io.micrometer.core.instrument.Timer;
// Prometheus imports removed - using basic Micrometer

import java.util.Random;

@RestController
public class PrometheusController {

    private final MeterRegistry meterRegistry;
    private final Counter prometheusRequests;
    private final Timer prometheusResponseTime;
    private final Random random = new Random();

    @Autowired
    public PrometheusController(MeterRegistry meterRegistry) {
        this.meterRegistry = meterRegistry;
        this.prometheusRequests = Counter.builder("prometheus_requests_total")
                .description("Total number of Prometheus requests")
                .register(meterRegistry);
        this.prometheusResponseTime = Timer.builder("prometheus_response_time_seconds")
                .description("Prometheus response time")
                .register(meterRegistry);
    }

    @GetMapping("/actuator/prometheus")
    public String prometheusMetrics() {
        prometheusRequests.increment();
        
        Timer.Sample sample = Timer.start();
        try {
            // Simulate some processing time
            Thread.sleep(random.nextInt(10) + 5);
            
            // Generate Prometheus format metrics
            StringBuilder metrics = new StringBuilder();
            
            // Add some basic metrics
            metrics.append("# HELP prometheus_requests_total Total number of Prometheus requests\n");
            metrics.append("# TYPE prometheus_requests_total counter\n");
            metrics.append("prometheus_requests_total ").append(prometheusRequests.count()).append("\n");
            
            metrics.append("# HELP prometheus_response_time_seconds Prometheus response time\n");
            metrics.append("# TYPE prometheus_response_time_seconds histogram\n");
            metrics.append("prometheus_response_time_seconds_bucket{le=\"0.005\"} 0\n");
            metrics.append("prometheus_response_time_seconds_bucket{le=\"0.01\"} 0\n");
            metrics.append("prometheus_response_time_seconds_bucket{le=\"0.025\"} 0\n");
            metrics.append("prometheus_response_time_seconds_bucket{le=\"0.05\"} 0\n");
            metrics.append("prometheus_response_time_seconds_bucket{le=\"0.1\"} 0\n");
            metrics.append("prometheus_response_time_seconds_bucket{le=\"0.25\"} 0\n");
            metrics.append("prometheus_response_time_seconds_bucket{le=\"0.5\"} 0\n");
            metrics.append("prometheus_response_time_seconds_bucket{le=\"1\"} 0\n");
            metrics.append("prometheus_response_time_seconds_bucket{le=\"2.5\"} 0\n");
            metrics.append("prometheus_response_time_seconds_bucket{le=\"5\"} 0\n");
            metrics.append("prometheus_response_time_seconds_bucket{le=\"10\"} 0\n");
            metrics.append("prometheus_response_time_seconds_bucket{le=\"+Inf\"} 0\n");
            metrics.append("prometheus_response_time_seconds_sum 0\n");
            metrics.append("prometheus_response_time_seconds_count 0\n");
            
            // Add JVM metrics
            Runtime runtime = Runtime.getRuntime();
            long totalMemory = runtime.totalMemory();
            long freeMemory = runtime.freeMemory();
            long usedMemory = totalMemory - freeMemory;
            
            metrics.append("# HELP jvm_memory_used_bytes Used JVM memory in bytes\n");
            metrics.append("# TYPE jvm_memory_used_bytes gauge\n");
            metrics.append("jvm_memory_used_bytes ").append(usedMemory).append("\n");
            
            metrics.append("# HELP jvm_memory_total_bytes Total JVM memory in bytes\n");
            metrics.append("# TYPE jvm_memory_total_bytes gauge\n");
            metrics.append("jvm_memory_total_bytes ").append(totalMemory).append("\n");
            
            // Add CPU metrics
            metrics.append("# HELP process_cpu_seconds_total Total CPU time used by the process\n");
            metrics.append("# TYPE process_cpu_seconds_total counter\n");
            metrics.append("process_cpu_seconds_total ").append(System.currentTimeMillis() / 1000.0).append("\n");
            
            return metrics.toString();
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
            return "# Error generating metrics\n";
        } finally {
            sample.stop(prometheusResponseTime);
        }
    }
}
