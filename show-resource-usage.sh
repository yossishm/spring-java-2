#!/bin/bash

echo "📊 Java vs .NET Resource Usage"
echo "=============================="
echo ""

# Check pod status
echo "🔍 Pod Status:"
kubectl get pods -n otel-system -l app=java-app
kubectl get pods -n otel-system -l app=dotnet-app
echo ""

# Check if metrics server is available
echo "📈 Resource Metrics:"
if kubectl top pods -n otel-system 2>/dev/null; then
    echo "✅ Metrics server is available"
else
    echo "⚠️  Metrics server not available - using Prometheus metrics instead"
    echo ""
    echo "🔍 Current Prometheus Metrics:"
    echo "Java App CPU:"
    curl -s "http://127.0.0.1:9090/api/v1/query?query=rate(process_cpu_seconds_total{job=\"java-app\"}[5m])*100" | jq '.data.result[0].value[1]' 2>/dev/null || echo "No data"
    
    echo "Java App Memory:"
    curl -s "http://127.0.0.1:9090/api/v1/query?query=process_resident_memory_bytes{job=\"java-app\"}" | jq '.data.result[0].value[1]' 2>/dev/null || echo "No data"
    
    echo ".NET App CPU:"
    curl -s "http://127.0.0.1:9090/api/v1/query?query=rate(process_cpu_seconds_total{job=\"dotnet-app\"}[5m])*100" | jq '.data.result[0].value[1]' 2>/dev/null || echo "No data"
    
    echo ".NET App Memory:"
    curl -s "http://127.0.0.1:9090/api/v1/query?query=process_resident_memory_bytes{job=\"dotnet-app\"}" | jq '.data.result[0].value[1]' 2>/dev/null || echo "No data"
fi

echo ""
echo "🎯 Dashboard Setup:"
echo "   1. Import: grafana-dashboard-java-vs-dotnet.json"
echo "   2. Set time range to 'Last 1 hour'"
echo "   3. Watch the comparison in real-time!"
echo ""
echo "📊 What you'll see:"
echo "   - CPU usage comparison (Java vs .NET)"
echo "   - Memory usage comparison"
echo "   - Virtual memory comparison"
echo "   - Request rate comparison"
echo ""
echo "🔄 Load patterns:"
echo "   - Baseline load (both apps)"
echo "   - Java-focused load"
echo "   - .NET-focused load"
echo "   - Heavy load on each app"
echo ""
echo "✨ The dashboard will show real-time differences!"
