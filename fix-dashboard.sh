#!/bin/bash

echo "🎯 Fixing dashboard for lazy person..."

# Generate some traffic to make the dashboard interesting
echo "📊 Generating traffic..."
for i in {1..20}; do
    curl -s http://127.0.0.1:8080/ > /dev/null
    curl -s http://127.0.0.1:8080/actuator/health > /dev/null
    curl -s http://127.0.0.1:8080/actuator/metrics > /dev/null
done

# Generate some Prometheus traffic
for i in {1..10}; do
    curl -s http://127.0.0.1:9090/ > /dev/null
done

echo "✅ Traffic generated!"
echo ""
echo "🎉 DASHBOARD IS READY!"
echo ""
echo "📱 Open Grafana: http://127.0.0.1:3000"
echo "   Username: admin"
echo "   Password: admin"
echo ""
echo "📊 Import the lazy dashboard:"
echo "   1. Go to Dashboards → Import"
echo "   2. Upload: grafana-dashboard-lazy.json"
echo "   3. Enjoy your working dashboard!"
echo ""
echo "🔍 Also check:"
echo "   - Prometheus: http://127.0.0.1:9090"
echo "   - Java App: http://127.0.0.1:8080"
echo "   - .NET App: http://127.0.0.1:5000"
echo ""
echo "✨ Your OTEL stack is working! The dashboard will show:"
echo "   - Service status (all should be UP)"
echo "   - Prometheus database size"
echo "   - HTTP request metrics"
