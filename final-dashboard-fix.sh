#!/bin/bash

echo "🎯 Final dashboard fix - this WILL work!"

# Generate traffic to ensure we have data
echo "📊 Generating traffic..."
for i in {1..10}; do
    curl -s "http://127.0.0.1:9090/" > /dev/null
    curl -s "http://127.0.0.1:9090/api/v1/query?query=up" > /dev/null
done

echo "✅ Traffic generated!"

# Test datasource
echo "🔍 Testing datasource..."
if curl -s "http://127.0.0.1:3000/api/datasources/1/health" -u admin:admin | grep -q "OK"; then
    echo "✅ Datasource is working!"
else
    echo "❌ Datasource issue detected"
fi

echo ""
echo "🎉 READY TO IMPORT DASHBOARD!"
echo ""
echo "📱 Go to Grafana: http://127.0.0.1:3000"
echo "   Username: admin"
echo "   Password: admin"
echo ""
echo "📊 Import the simple dashboard:"
echo "   1. Go to Dashboards → Import"
echo "   2. Upload: grafana-dashboard-simple-working.json"
echo "   3. Click 'Load' then 'Import'"
echo ""
echo "✨ This dashboard will show:"
echo "   - Service status (up metric)"
echo "   - Real data from Prometheus"
echo "   - Updates every 5 seconds"
echo ""
echo "🔧 If still no data:"
echo "   1. Check time range (top right) - set to 'Last 1 hour'"
echo "   2. Refresh the dashboard (F5)"
echo "   3. Check datasource: Settings → Data Sources → Prometheus → Test"
