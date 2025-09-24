package hello;

import io.micrometer.core.instrument.Counter;
import io.micrometer.core.instrument.MeterRegistry;
import io.micrometer.core.instrument.Timer;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.Random;


@RestController
@RequestMapping("/api/metrics")
public class MetricsController {

    private final Counter requestCounter;
    private final Timer responseTimer;
    private final Random random = new Random();

    @Autowired
    public MetricsController(MeterRegistry meterRegistry) {
        this.requestCounter = Counter.builder("custom_requests_total")
                .description("Total number of custom requests")
                .register(meterRegistry);
        
        this.responseTimer = Timer.builder("custom_response_time")
                .description("Custom response time")
                .register(meterRegistry);
    }

    @GetMapping("/test")
    public String testMetrics() {
        requestCounter.increment();
        
        Timer.Sample sample = Timer.start();
        try {
            // Simulate some work
            Thread.sleep(50L + random.nextInt(100));
            return "Metrics test completed";
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
            return "Metrics test interrupted";
        } finally {
            sample.stop(responseTimer);
        }
    }

    @GetMapping("/counter")
    public String incrementCounter(@RequestParam(defaultValue = "1") int value) {
        requestCounter.increment(value);
        return "Counter incremented by " + value;
    }

    @GetMapping("/slow")
    public String slowEndpoint() {
        requestCounter.increment();
        
        Timer.Sample sample = Timer.start();
        try {
            // Simulate slow operation
            Thread.sleep(500L + random.nextInt(1000));
            return "Slow operation completed";
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
            return "Slow operation interrupted";
        } finally {
            sample.stop(responseTimer);
        }
    }
}
