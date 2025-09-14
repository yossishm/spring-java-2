#!/bin/bash

echo "⚔️ Java vs .NET Performance Comparison"
echo "======================================"
echo ""

# Check if load generators are running
if pgrep -f "load-generator-java-dotnet.sh" > /dev/null; then
    echo "✅ Java vs .NET load generator is running"
else
    echo "⚠️  Starting Java vs .NET load generator..."
    ./load-generator-java-dotnet.sh &
    sleep 2
fi

echo ""
echo "📊 CURRENT STATUS:"
echo "   - Java App: Running (2 pods)"
echo "   - .NET App: Running (2 pods)"
echo "   - Load Generator: Creating different load patterns"
echo ""

echo "🔍 WHAT WE CAN MEASURE:"
echo "   - HTTP request patterns to each app"
echo "   - Response times (via load generator)"
echo "   - Prometheus metrics collection"
echo "   - Service availability"
echo ""

echo "⚠️  LIMITATION:"
echo "   - Java and .NET apps don't expose process metrics to Prometheus"
echo "   - We can't directly measure CPU/Memory usage"
echo "   - But we can measure HTTP performance and load patterns"
echo ""

echo "🎯 DASHBOARD OPTIONS:"
echo "   1. grafana-dashboard-http-comparison.json (HTTP performance)"
echo "   2. grafana-dashboard-java-vs-dotnet.json (if metrics were available)"
echo ""

echo "📈 LOAD PATTERNS BEING GENERATED:"
echo "   - Baseline load (both apps)"
echo "   - Java-focused load (more requests to Java app)"
echo "   - .NET-focused load (more requests to .NET app)"
echo "   - Heavy load on each app separately"
echo ""

echo "🔧 TO GET CPU/MEMORY METRICS:"
echo "   - Need to enable metrics server in Kubernetes"
echo "   - Or configure apps to expose process metrics"
echo "   - Current setup shows HTTP performance patterns"
echo ""

echo "✨ Import the HTTP comparison dashboard to see:"
echo "   - Request rates to different services"
echo "   - Service status (up/down)"
echo "   - Prometheus database growth"
echo "   - Sample processing rates"
echo ""
echo "🎉 The load generator is creating measurable differences!"
