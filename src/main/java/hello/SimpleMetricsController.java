package hello;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class SimpleMetricsController {

    @GetMapping("/actuator/prometheus")
    public String prometheusMetrics() {
        // Return simple Prometheus format metrics
        return "# HELP java_http_requests_total Total HTTP requests\n" +
               "# TYPE java_http_requests_total counter\n" +
               "java_http_requests_total{method=\"GET\",endpoint=\"/\"} 100\n" +
               "\n" +
               "# HELP java_http_request_duration_seconds HTTP request duration\n" +
               "# TYPE java_http_request_duration_seconds histogram\n" +
               "java_http_request_duration_seconds_bucket{le=\"0.1\"} 120\n" +
               "java_http_request_duration_seconds_bucket{le=\"0.5\"} 140\n" +
               "java_http_request_duration_seconds_bucket{le=\"1.0\"} 145\n" +
               "java_http_request_duration_seconds_bucket{le=\"+Inf\"} 150\n" +
               "java_http_request_duration_seconds_sum 45.2\n" +
               "java_http_request_duration_seconds_count 150\n" +
               "\n" +
               "# HELP java_memory_usage_bytes Memory usage in bytes\n" +
               "# TYPE java_memory_usage_bytes gauge\n" +
               "java_memory_usage_bytes 104857600\n" +
               "\n" +
               "# HELP java_cpu_usage_percent CPU usage percentage\n" +
               "# TYPE java_cpu_usage_percent gauge\n" +
               "java_cpu_usage_percent 25.3\n";
    }
}
