#!/bin/bash

echo "🚀 REAL-TIME LOAD MONITORING SETUP"
echo "=================================="
echo ""

# Check if load generator is running
if pgrep -f "generate-load.sh" > /dev/null; then
    echo "✅ Load generator is running in background"
else
    echo "⚠️  Starting load generator..."
    ./generate-load.sh &
    sleep 2
fi

echo ""
echo "📊 LOAD GENERATOR STATUS:"
echo "   - Generating continuous traffic to all services"
echo "   - HTTP requests to Prometheus, Java app, .NET app"
echo "   - Random delays to create realistic patterns"
echo ""

echo "🎯 DASHBOARD SETUP:"
echo "   1. Open Grafana: http://127.0.0.1:3000 (admin/admin)"
echo "   2. Import: grafana-dashboard-realtime-load.json"
echo "   3. Set time range to 'Last 5 minutes'"
echo "   4. Watch the metrics update in real-time!"
echo ""

echo "📈 WHAT YOU'LL SEE:"
echo "   - HTTP requests/sec (should be constantly updating)"
echo "   - Service status (Prometheus UP, others DOWN)"
echo "   - Database size (growing as data is collected)"
echo "   - Samples processed (increasing with each request)"
echo ""

echo "🔄 REAL-TIME UPDATES:"
echo "   - Dashboard refreshes every 5 seconds"
echo "   - Load generator creates new requests continuously"
echo "   - Metrics update in real-time as you watch"
echo ""

echo "🛑 TO STOP LOAD GENERATOR:"
echo "   Run: pkill -f generate-load.sh"
echo ""

echo "✨ Your real-time monitoring is ready!"
echo "   Import the dashboard and watch the metrics live!"
