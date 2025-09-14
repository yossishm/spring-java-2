#!/bin/bash

# Build script for offline distroless Docker images
# This script builds the base runtime once, then builds the application

set -e

echo "Building offline distroless Docker images..."

# Build the base runtime image (only needs to be done once)
echo "Step 1: Building base runtime image..."
docker build -f Dockerfile.base-runtime -t java-runtime:21-alpine-distroless .

echo "Base runtime image built successfully!"

# Build the application image using the base runtime
echo "Step 2: Building application image..."
docker build -f Dockerfile.alpine-distroless-from-base -t spring-app:distroless-offline .

echo "Application image built successfully!"

# Alternative: Build the prebuilt version (doesn't require separate base image)
echo "Step 3: Building alternative prebuilt version..."
docker build -f Dockerfile.alpine-distroless-prebuilt -t spring-app:distroless-prebuilt .

echo "All images built successfully!"
echo ""
echo "Available images:"
echo "  - java-runtime:21-alpine-distroless (base runtime)"
echo "  - spring-app:distroless-offline (using base runtime)"
echo "  - spring-app:distroless-prebuilt (self-contained)"
echo ""
echo "To run the application:"
echo "  docker run -p 8080:8080 spring-app:distroless-offline"
echo "  or"
echo "  docker run -p 8080:8080 spring-app:distroless-prebuilt"
