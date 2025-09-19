#!/bin/bash

echo "🚀 Starting load generator for real-time metrics..."
echo "   This will continuously generate traffic to create visible metrics"
echo "   Press Ctrl+C to stop"
echo ""

# Function to generate load
generate_load() {
    while true; do
        # Generate traffic to different endpoints
        # Generate traffic to Java app (using LoadBalancer port)
        curl -s "http://127.0.0.1:java/" > /dev/null
        #curl -s "http://127.0.0.1:30543/actuator/health" > /dev/null
        #curl -s "http://127.0.0.1:30543/actuator/metrics" > /dev/null
        
        # Generate traffic to .NET app (using LoadBalancer port)
        curl -s "http://127.0.0.1:dotnet/" > /dev/null
        #curl -s "http://127.0.0.1:32517/health/ready" > /dev/null
        
        
        # Random delay between 0.1 and 0.5 seconds
        #sleep $(echo "scale=2; $RANDOM/65536*0.4+0.1" | bc -l 2>/dev/null || echo "0.2")
        
        # Show progress every 50 requests
        if [ $((++count % 50)) -eq 0 ]; then
            echo "📊 Generated $count requests... (Press Ctrl+C to stop)"
        fi
    done
}

# Start the load generator
count=0
generate_load
