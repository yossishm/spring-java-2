#!/bin/bash

echo "⚖️ Balanced Java vs .NET Load Generator"
echo "   Creating steady, equal load on both applications"
echo "   Press Ctrl+C to stop"
echo ""

# Configuration
REQUESTS_PER_SECOND=5
JAVA_URL="http://127.0.0.1:8080"
DOTNET_URL="http://127.0.0.1:5000"

# Function to generate balanced load
generate_balanced_load() {
    local cycle=$1
    echo "🔄 Load Cycle $cycle - Generating balanced load..."
    
    # Generate equal load to both applications
    for i in {1..5}; do
        # Java requests - call main endpoint
        curl -s "$JAVA_URL/" > /dev/null &
        
        # .NET requests - call main endpoint
        curl -s "$DOTNET_URL/" > /dev/null &
        
        # Small delay to spread requests
        sleep 0.2
    done
    
    # Wait for all background requests to complete
    wait
    
    echo "   ☕ Java: 5 requests sent"
    echo "   🔷 .NET: 5 requests sent"
    echo "   📊 Total: 10 requests in this cycle"
}

# Function to generate light burst load
generate_burst_load() {
    local cycle=$1
    echo "🚀 Load Cycle $cycle - Generating burst load..."
    
    # Generate burst load to both applications equally
    for i in {1..10}; do
        # Java requests - call main endpoint
        curl -s "$JAVA_URL/" > /dev/null &
        
        # .NET requests - call main endpoint
        curl -s "$DOTNET_URL/" > /dev/null &
    done
    
    # Wait for all background requests to complete
    wait
    
    echo "   ☕ Java: 10 requests sent"
    echo "   🔷 .NET: 10 requests sent"
    echo "   📊 Total: 20 requests in this cycle"
}

# Main load generation loop
cycle=0
while true; do
    cycle=$((cycle + 1))
    
    # Alternate between steady and burst load every 4 cycles
    if [ $((cycle % 4)) -eq 0 ]; then
        generate_burst_load $cycle
        sleep 3  # Longer pause after burst
    else
        generate_balanced_load $cycle
        sleep 2  # Regular pause
    fi
    
    echo "   ⏱️  Completed cycle $cycle"
    echo "   📈 Check your dashboard at http://localhost:3000/d/java-vs-dotnet-fixed"
    echo ""
done
