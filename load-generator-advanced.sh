#!/bin/bash

echo "🎯 Advanced Load Generator - Real-time Metrics"
echo "   This creates realistic load patterns you can see in Grafana"
echo "   Press Ctrl+C to stop"
echo ""

# Function to generate burst load
burst_load() {
    echo "💥 Generating burst load..."
    for i in {1..20}; do
        curl -s "http://127.0.0.1:9090/" > /dev/null &
        curl -s "http://127.0.0.1:9090/api/v1/query?query=up" > /dev/null &
        curl -s "http://127.0.0.1:8080/" > /dev/null &
        curl -s "http://127.0.0.1:5000/" > /dev/null &
    done
    wait
    echo "✅ Burst load complete"
}

# Function to generate steady load
steady_load() {
    echo "📈 Generating steady load..."
    for i in {1..10}; do
        curl -s "http://127.0.0.1:9090/" > /dev/null
        curl -s "http://127.0.0.1:9090/api/v1/query?query=prometheus_http_requests_total" > /dev/null
        curl -s "http://127.0.0.1:8080/actuator/health" > /dev/null
        curl -s "http://127.0.0.1:5000/health/ready" > /dev/null
        sleep 0.5
    done
    echo "✅ Steady load complete"
}

# Function to generate random load
random_load() {
    echo "🎲 Generating random load..."
    for i in {1..15}; do
        # Random endpoint selection
        endpoints=(
            "http://127.0.0.1:9090/"
            "http://127.0.0.1:9090/api/v1/query?query=up"
            "http://127.0.0.1:9090/api/v1/query?query=prometheus_http_requests_total"
            "http://127.0.0.1:8080/"
            "http://127.0.0.1:8080/actuator/health"
            "http://127.0.0.1:5000/"
            "http://127.0.0.1:5000/health/ready"
        )
        
        # Pick random endpoint
        endpoint=${endpoints[$RANDOM % ${#endpoints[@]}]}
        curl -s "$endpoint" > /dev/null
        
        # Random delay
        sleep $(echo "scale=2; $RANDOM/65536*2+0.1" | bc -l 2>/dev/null || echo "0.5")
    done
    echo "✅ Random load complete"
}

# Main load generation loop
count=0
while true; do
    echo ""
    echo "🔄 Load Generation Cycle $((++count))"
    
    # Generate different types of load
    burst_load
    sleep 2
    steady_load
    sleep 2
    random_load
    sleep 3
    
    echo "📊 Total cycles completed: $count"
    echo "   Check your Grafana dashboard to see the metrics!"
    echo "   Press Ctrl+C to stop"
done
