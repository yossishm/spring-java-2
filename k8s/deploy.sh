#!/bin/bash

echo "🚀 Deploying OpenTelemetry Stack to Kubernetes..."

# Check if kubectl is available
if ! command -v kubectl &> /dev/null; then
    echo "❌ kubectl is not installed. Please install kubectl first."
    exit 1
fi

# Check if cluster is accessible
if ! kubectl cluster-info &> /dev/null; then
    echo "❌ Cannot connect to Kubernetes cluster. Please check your kubeconfig."
    exit 1
fi

# Build Docker images first
echo "📦 Building Docker images..."
docker build -t spring-java-2-java-app:latest ..
docker build -t spring-java-2-dotnet-app:latest ../dotnet-spring-equivalent

# Load images into kind/minikube if needed
if command -v kind &> /dev/null; then
    echo "📥 Loading images into kind cluster..."
    kind load docker-image spring-java-2-java-app:latest
    kind load docker-image spring-java-2-dotnet-app:latest
elif command -v minikube &> /dev/null; then
    echo "📥 Loading images into minikube cluster..."
    minikube image load spring-java-2-java-app:latest
    minikube image load spring-java-2-dotnet-app:latest
fi

# Create namespace
echo "🏗️  Creating namespace..."
kubectl apply -f namespace.yaml

# Deploy OTEL Collector
echo "📡 Deploying OTEL Collector..."
kubectl apply -f otel-collector/

# Deploy Prometheus
echo "📊 Deploying Prometheus..."
kubectl apply -f prometheus/

# Deploy Jaeger
echo "🔍 Deploying Jaeger..."
kubectl apply -f jaeger.yaml

# Deploy Grafana
echo "📈 Deploying Grafana..."
kubectl apply -f grafana/

# Deploy Applications
echo "☕ Deploying Java Application..."
kubectl apply -f apps/java-app.yaml

echo "🔷 Deploying .NET Application..."
kubectl apply -f apps/dotnet-app.yaml

# Deploy Ingress
echo "🌐 Deploying Ingress..."
kubectl apply -f ingress/

# Wait for deployments
echo "⏳ Waiting for deployments to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/otel-collector -n otel-system
kubectl wait --for=condition=available --timeout=300s deployment/prometheus -n otel-system
kubectl wait --for=condition=available --timeout=300s deployment/grafana -n otel-system
kubectl wait --for=condition=available --timeout=300s deployment/jaeger -n otel-system
kubectl wait --for=condition=available --timeout=300s deployment/java-app -n otel-system
kubectl wait --for=condition=available --timeout=300s deployment/dotnet-app -n otel-system

echo ""
echo "🎉 OpenTelemetry Stack deployed successfully!"
echo ""
echo "📊 Access your services:"

# Get service URLs
if command -v minikube &> /dev/null; then
    echo "   Grafana Dashboard: $(minikube service grafana-lb -n otel-system --url)"
    echo "   Prometheus:        $(minikube service prometheus-lb -n otel-system --url)"
    echo "   Java App:          $(minikube service java-app-lb -n otel-system --url)"
    echo "   .NET App:          $(minikube service dotnet-app-lb -n otel-system --url)"
elif command -v kind &> /dev/null; then
    echo "   Grafana Dashboard: http://localhost:3000 (port-forward)"
    echo "   Prometheus:        http://localhost:9090 (port-forward)"
    echo "   Java App:          http://localhost:8080 (port-forward)"
    echo "   .NET App:          http://localhost:5000 (port-forward)"
    echo ""
    echo "   To access services, run:"
    echo "   kubectl port-forward -n otel-system svc/grafana-lb 3000:3000 &"
    echo "   kubectl port-forward -n otel-system svc/prometheus-lb 9090:9090 &"
    echo "   kubectl port-forward -n otel-system svc/java-app-lb 8080:8080 &"
    echo "   kubectl port-forward -n otel-system svc/dotnet-app-lb 5000:5000 &"
else
    echo "   Use 'kubectl get svc -n otel-system' to see service endpoints"
fi

echo ""
echo "🧪 Generate some test metrics:"
echo "   curl <java-app-url>/api/metrics/test"
echo "   curl <dotnet-app-url>/api/metrics/test"
echo ""
echo "📖 See K8S-OTEL-SETUP-GUIDE.md for detailed instructions"
