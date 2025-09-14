#!/bin/bash

echo "⚔️ Java vs .NET Load Generator"
echo "   This will create different load patterns to compare performance"
echo "   Press Ctrl+C to stop"
echo ""

# Function to generate Java app load
java_load() {
    echo "☕ Generating Java app load..."
    for i in {1..20}; do
        curl -s "http://127.0.0.1:8080/" > /dev/null &
        curl -s "http://127.0.0.1:8080/actuator/health" > /dev/null &
        curl -s "http://127.0.0.1:8080/actuator/metrics" > /dev/null &
    done
    wait
}

# Function to generate .NET app load
dotnet_load() {
    echo "🔷 Generating .NET app load..."
    for i in {1..20}; do
        curl -s "http://127.0.0.1:5000/" > /dev/null &
        curl -s "http://127.0.0.1:5000/health/ready" > /dev/null &
        curl -s "http://127.0.0.1:5000/health/live" > /dev/null &
    done
    wait
}

# Function to generate mixed load
mixed_load() {
    echo "🔄 Generating mixed load..."
    for i in {1..10}; do
        # Java requests
        curl -s "http://127.0.0.1:8080/" > /dev/null &
        curl -s "http://127.0.0.1:8080/actuator/health" > /dev/null &
        
        # .NET requests
        curl -s "http://127.0.0.1:5000/" > /dev/null &
        curl -s "http://127.0.0.1:5000/health/ready" > /dev/null &
        
        sleep 0.1
    done
    wait
}

# Function to generate heavy Java load
heavy_java_load() {
    echo "🔥 Generating HEAVY Java load..."
    for i in {1..50}; do
        curl -s "http://127.0.0.1:8080/" > /dev/null &
        curl -s "http://127.0.0.1:8080/actuator/health" > /dev/null &
        curl -s "http://127.0.0.1:8080/actuator/metrics" > /dev/null &
    done
    wait
}

# Function to generate heavy .NET load
heavy_dotnet_load() {
    echo "💥 Generating HEAVY .NET load..."
    for i in {1..50}; do
        curl -s "http://127.0.0.1:5000/" > /dev/null &
        curl -s "http://127.0.0.1:5000/health/ready" > /dev/null &
        curl -s "http://127.0.0.1:5000/health/live" > /dev/null &
    done
    wait
}

# Main load generation loop
count=0
while true; do
    echo ""
    echo "🔄 Load Generation Cycle $((++count))"
    
    # Generate different load patterns
    case $((count % 6)) in
        0)
            echo "📊 Phase: Baseline (light load)"
            mixed_load
            sleep 3
            ;;
        1)
            echo "☕ Phase: Java Focus"
            java_load
            sleep 2
            ;;
        2)
            echo "🔷 Phase: .NET Focus"
            dotnet_load
            sleep 2
            ;;
        3)
            echo "🔥 Phase: Heavy Java Load"
            heavy_java_load
            sleep 2
            ;;
        4)
            echo "💥 Phase: Heavy .NET Load"
            heavy_dotnet_load
            sleep 2
            ;;
        5)
            echo "⚖️ Phase: Balanced Load"
            mixed_load
            sleep 3
            ;;
    esac
    
    echo "📈 Total cycles completed: $count"
    echo "   Check your Java vs .NET dashboard to see the differences!"
    echo "   Press Ctrl+C to stop"
done
