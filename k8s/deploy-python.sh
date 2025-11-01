#!/bin/bash

# Deploy Python Spring Equivalent to Kubernetes
# This script builds the Docker image and deploys to Kubernetes

set -e

echo "🚀 Deploying Python Spring Equivalent to Kubernetes..."

# Configuration
IMAGE_NAME="spring-java-2-python-app"
IMAGE_TAG="latest"
NAMESPACE="otel-system"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if we're in the right directory
if [ ! -f "python-spring-equivalent/requirements.txt" ]; then
    print_error "Please run this script from the project root directory"
    exit 1
fi

# Build Docker image
print_status "Building Docker image..."
cd python-spring-equivalent
docker build -f Dockerfile.k8s -t ${IMAGE_NAME}:${IMAGE_TAG} .
if [ $? -eq 0 ]; then
    print_success "Docker image built successfully"
else
    print_error "Failed to build Docker image"
    exit 1
fi

# Go back to project root
cd ..

# Check if kubectl is available
if ! command -v kubectl &> /dev/null; then
    print_error "kubectl is not installed or not in PATH"
    exit 1
fi

# Check if namespace exists
if ! kubectl get namespace ${NAMESPACE} &> /dev/null; then
    print_warning "Namespace ${NAMESPACE} does not exist. Creating it..."
    kubectl create namespace ${NAMESPACE}
    print_success "Namespace ${NAMESPACE} created"
fi

# Deploy the application
print_status "Deploying Python application to Kubernetes..."
kubectl apply -f k8s/apps/python-app.yaml

if [ $? -eq 0 ]; then
    print_success "Python application deployed successfully"
else
    print_error "Failed to deploy Python application"
    exit 1
fi

# Wait for deployment to be ready
print_status "Waiting for deployment to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/python-app -n ${NAMESPACE}

if [ $? -eq 0 ]; then
    print_success "Deployment is ready"
else
    print_warning "Deployment may not be fully ready yet"
fi

# Show deployment status
print_status "Deployment status:"
kubectl get deployments -n ${NAMESPACE} | grep python-app

print_status "Pod status:"
kubectl get pods -n ${NAMESPACE} | grep python-app

print_status "Service status:"
kubectl get services -n ${NAMESPACE} | grep python-app

# Show logs
print_status "Recent logs from Python application:"
kubectl logs -l app=python-app -n ${NAMESPACE} --tail=20

# Port forward for testing (optional)
print_status "To test the application locally, run:"
echo "kubectl port-forward service/python-app 8080:8080 -n ${NAMESPACE}"
echo ""
echo "Then visit: http://localhost:8080"

print_success "Python Spring Equivalent deployment completed!"



