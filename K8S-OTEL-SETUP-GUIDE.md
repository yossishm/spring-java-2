# Kubernetes OpenTelemetry Setup Guide

This guide will help you set up a complete OpenTelemetry stack on Kubernetes to collect and visualize metrics from both your .NET and Java applications.

## Why Kubernetes?

✅ **No Docker file sharing issues** - K8s handles configuration via ConfigMaps and Secrets  
✅ **Production-ready** - Proper service discovery, health checks, and scaling  
✅ **Easier management** - Standard K8s patterns and tools  
✅ **Better networking** - Native service mesh capabilities  
✅ **Scalability** - Easy horizontal scaling of components  

## Prerequisites

### Required Tools
- **kubectl** - Kubernetes command-line tool
- **Docker** - For building application images
- **Kubernetes cluster** - One of:
  - **minikube** (local development)
  - **kind** (local development)
  - **Docker Desktop** (with Kubernetes enabled)
  - **Cloud cluster** (GKE, EKS, AKS)

### Quick Setup Options

#### Option 1: minikube (Recommended for local development)
```bash
# Install minikube
brew install minikube  # macOS
# or
curl -LO https://storage.googleapis.com/minikube/releases/latest/minikube-linux-amd64
sudo install minikube-linux-amd64 /usr/local/bin/minikube

# Start minikube
minikube start --memory=4096 --cpus=2
```

#### Option 2: kind (Kubernetes in Docker)
```bash
# Install kind
brew install kind  # macOS
# or
curl -Lo ./kind https://kind.sigs.k8s.io/dl/v0.20.0/kind-linux-amd64
chmod +x ./kind
sudo mv ./kind /usr/local/bin/kind

# Create cluster
kind create cluster --name otel-cluster
```

#### Option 3: Docker Desktop
1. Enable Kubernetes in Docker Desktop settings
2. Wait for cluster to be ready

## Quick Start

### 1. Deploy the Stack
```bash
# Deploy everything with one command
./k8s/deploy.sh
```

### 2. Access Services

#### With minikube:
```bash
# Get service URLs
minikube service grafana-lb -n otel-system --url
minikube service prometheus-lb -n otel-system --url
minikube service java-app-lb -n otel-system --url
minikube service dotnet-app-lb -n otel-system --url
```

#### With kind or other clusters:
```bash
# Port forward to access services
kubectl port-forward -n otel-system svc/grafana-lb 3000:3000 &
kubectl port-forward -n otel-system svc/prometheus-lb 9090:9090 &
kubectl port-forward -n otel-system svc/java-app-lb 8080:8080 &
kubectl port-forward -n otel-system svc/dotnet-app-lb 5000:5000 &
```

### 3. Access Points
- **Grafana Dashboard**: http://localhost:3000 (admin/admin)
- **Prometheus**: http://localhost:9090
- **Java App**: http://localhost:8080
- **.NET App**: http://localhost:5000

## Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Java App      │    │   .NET App      │    │   OTEL Collector│
│   (2 replicas)  │    │   (2 replicas)  │    │   (ConfigMap)   │
└─────────┬───────┘    └─────────┬───────┘    └─────────┬───────┘
          │                      │                      │
          └──────────────────────┼──────────────────────┘
                                 │
                    ┌─────────────┴─────────────┐
                    │                           │
            ┌───────▼───────┐           ┌───────▼───────┐
            │  Prometheus   │           │    Jaeger     │
            │  (ConfigMap)  │           │  (Deployment) │
            └───────┬───────┘           └───────────────┘
                    │
            ┌───────▼───────┐
            │    Grafana    │
            │  (ConfigMap)  │
            └───────────────┘
```

## Manual Deployment

If you prefer to deploy step by step:

```bash
# 1. Create namespace
kubectl apply -f k8s/namespace.yaml

# 2. Deploy OTEL Collector
kubectl apply -f k8s/otel-collector/

# 3. Deploy Prometheus
kubectl apply -f k8s/prometheus/

# 4. Deploy Jaeger
kubectl apply -f k8s/jaeger.yaml

# 5. Deploy Grafana
kubectl apply -f k8s/grafana/

# 6. Build and deploy applications
docker build -t spring-java-2-java-app:latest .
docker build -t spring-java-2-dotnet-app:latest ./dotnet-spring-equivalent

# Load images into cluster (for minikube/kind)
minikube image load spring-java-2-java-app:latest
minikube image load spring-java-2-dotnet-app:latest

# Deploy applications
kubectl apply -f k8s/apps/

# Deploy ingress
kubectl apply -f k8s/ingress/
```

## Generate Test Metrics

```bash
# Get service URLs
JAVA_URL=$(minikube service java-app-lb -n otel-system --url)
DOTNET_URL=$(minikube service dotnet-app-lb -n otel-system --url)

# Generate metrics
curl $JAVA_URL/api/metrics/test
curl $DOTNET_URL/api/metrics/test

# Generate load
for i in {1..10}; do
  curl $JAVA_URL/api/metrics/slow &
  curl $DOTNET_URL/api/metrics/slow &
done
wait
```

## Monitoring and Troubleshooting

### Check Pod Status
```bash
kubectl get pods -n otel-system
kubectl get svc -n otel-system
```

### View Logs
```bash
kubectl logs -n otel-system deployment/otel-collector
kubectl logs -n otel-system deployment/java-app
kubectl logs -n otel-system deployment/dotnet-app
```

### Check Metrics Collection
```bash
# Check Prometheus targets
kubectl port-forward -n otel-system svc/prometheus 9090:9090
# Then visit http://localhost:9090/targets

# Check OTEL Collector metrics
kubectl port-forward -n otel-system svc/otel-collector 8889:8889
# Then visit http://localhost:8889/metrics
```

### Scale Applications
```bash
# Scale Java app
kubectl scale deployment java-app -n otel-system --replicas=3

# Scale .NET app
kubectl scale deployment dotnet-app -n otel-system --replicas=3
```

## Configuration Details

### OpenTelemetry Collector
- **ConfigMap**: `k8s/otel-collector/configmap.yaml`
- **Receives**: OTLP data on ports 4317 (gRPC) and 4318 (HTTP)
- **Exports**: Metrics to Prometheus, traces to Jaeger
- **Resources**: 256Mi-512Mi memory, 100m-500m CPU

### Prometheus
- **ConfigMap**: `k8s/prometheus/configmap.yaml`
- **Scrapes**: OTEL Collector, Java app, .NET app
- **Storage**: EmptyDir (ephemeral)
- **Resources**: 512Mi-1Gi memory, 200m-1000m CPU

### Grafana
- **ConfigMaps**: Datasources and dashboards
- **Pre-configured**: Prometheus datasource, custom dashboard
- **Storage**: EmptyDir (ephemeral)
- **Resources**: 256Mi-512Mi memory, 100m-500m CPU

### Applications
- **Java**: Spring Boot with Actuator metrics
- **.NET**: ASP.NET Core with OpenTelemetry
- **Replicas**: 2 each for high availability
- **Health Checks**: Liveness and readiness probes

## Cleanup

```bash
# Remove all resources
kubectl delete namespace otel-system

# Or remove individual components
kubectl delete -f k8s/ingress/
kubectl delete -f k8s/apps/
kubectl delete -f k8s/grafana/
kubectl delete -f k8s/prometheus/
kubectl delete -f k8s/otel-collector/
kubectl delete -f k8s/jaeger.yaml
kubectl delete -f k8s/namespace.yaml
```

## Production Considerations

### Persistent Storage
For production, replace `emptyDir` with persistent volumes:

```yaml
# Example for Prometheus
volumeClaimTemplates:
- metadata:
    name: storage
  spec:
    accessModes: ["ReadWriteOnce"]
    resources:
      requests:
        storage: 10Gi
```

### Resource Limits
Adjust resource requests and limits based on your cluster capacity and requirements.

### Security
- Use Kubernetes secrets for sensitive configuration
- Enable RBAC for service accounts
- Use network policies for traffic isolation

### Monitoring
- Set up alerting rules in Prometheus
- Configure Grafana alerts
- Monitor cluster resources

## Next Steps

1. **Add more custom metrics** to your applications
2. **Set up alerting** for critical metrics
3. **Configure log aggregation** using OTEL collector
4. **Add distributed tracing** across services
5. **Implement service mesh** (Istio) for advanced observability
6. **Set up CI/CD** for automated deployments

## Useful Commands

```bash
# Watch pod status
kubectl get pods -n otel-system -w

# Describe a pod
kubectl describe pod <pod-name> -n otel-system

# Execute into a pod
kubectl exec -it <pod-name> -n otel-system -- /bin/sh

# View events
kubectl get events -n otel-system --sort-by='.lastTimestamp'

# Check resource usage
kubectl top pods -n otel-system
kubectl top nodes
```
