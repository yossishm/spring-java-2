#!/bin/bash

echo "🚀 Starting load generator for real-time metrics..."
echo "   This will continuously generate traffic to create visible metrics"
echo "   Press Ctrl+C to stop"
echo ""

# Function to generate load
generate_load() {
    while true; do
        # Generate traffic to different endpoints
        curl -s "http://127.0.0.1:9090/" > /dev/null
        curl -s "http://127.0.0.1:9090/api/v1/query?query=up" > /dev/null
        curl -s "http://127.0.0.1:9090/api/v1/query?query=prometheus_http_requests_total" > /dev/null
        curl -s "http://127.0.0.1:9090/api/v1/query?query=prometheus_tsdb_symbol_table_size_bytes" > /dev/null
        
        # Generate traffic to Java app (even though metrics endpoint doesn't work)
        curl -s "http://127.0.0.1:8080/" > /dev/null
        curl -s "http://127.0.0.1:8080/actuator/health" > /dev/null
        curl -s "http://127.0.0.1:8080/actuator/metrics" > /dev/null
        
        # Generate traffic to .NET app
        curl -s "http://127.0.0.1:5000/" > /dev/null
        curl -s "http://127.0.0.1:5000/health/ready" > /dev/null
        
        # Generate some Grafana traffic
        curl -s "http://127.0.0.1:3000/api/health" > /dev/null
        
        # Random delay between 0.1 and 0.5 seconds
        sleep $(echo "scale=2; $RANDOM/65536*0.4+0.1" | bc -l 2>/dev/null || echo "0.2")
        
        # Show progress every 50 requests
        if [ $((++count % 50)) -eq 0 ]; then
            echo "📊 Generated $count requests... (Press Ctrl+C to stop)"
        fi
    done
}

# Start the load generator
count=0
generate_load
