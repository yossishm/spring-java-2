#!/bin/bash

# Container-based build script for GraalVM Native Image (OSS Community Edition)
# Supports both Docker and Podman - automatically detects which is available
# This script builds a native executable using containers, no local GraalVM installation needed

set -e

# Detect container runtime (Docker or Podman)
if command -v podman &> /dev/null; then
    CONTAINER_CMD="podman"
    echo "Using Podman as container runtime"
elif command -v docker &> /dev/null; then
    CONTAINER_CMD="docker"
    echo "Using Docker as container runtime"
else
    echo "ERROR: Neither Docker nor Podman is installed."
    echo "Please install Docker (https://www.docker.com/) or Podman (https://podman.io/)"
    exit 1
fi

echo "=========================================="
echo "Building Native Image with ${CONTAINER_CMD} (GraalVM OSS)"
echo "=========================================="

# Build container image
IMAGE_NAME="${IMAGE_NAME:-spring-boot-native}"
IMAGE_TAG="${IMAGE_TAG:-latest}"
DOCKERFILE="${DOCKERFILE:-Dockerfile.native}"

echo "Building container image: ${IMAGE_NAME}:${IMAGE_TAG}"
echo "Using Dockerfile: ${DOCKERFILE}"
echo ""

# Build the container image
${CONTAINER_CMD} build -f "${DOCKERFILE}" -t "${IMAGE_NAME}:${IMAGE_TAG}" .

if [ $? -eq 0 ]; then
    echo ""
    echo "=========================================="
    echo "✓ Container image built successfully!"
    echo "=========================================="
    echo "Image: ${IMAGE_NAME}:${IMAGE_TAG}"
    echo ""
    echo "To run the container:"
    if [ "$CONTAINER_CMD" = "podman" ]; then
        echo "  podman run -p 8080:8080 ${IMAGE_NAME}:${IMAGE_TAG}"
    else
        echo "  docker run -p 8080:8080 ${IMAGE_NAME}:${IMAGE_TAG}"
    fi
    echo ""
    echo "To extract the native executable from the image:"
    if [ "$CONTAINER_CMD" = "podman" ]; then
        echo "  podman create --name temp-container ${IMAGE_NAME}:${IMAGE_TAG}"
        echo "  podman cp temp-container:/app/app ./target/gs-spring-boot-docker-native"
        echo "  podman rm temp-container"
    else
        echo "  docker create --name temp-container ${IMAGE_NAME}:${IMAGE_TAG}"
        echo "  docker cp temp-container:/app/app ./target/gs-spring-boot-docker-native"
        echo "  docker rm temp-container"
    fi
    echo ""
    echo "Image size:"
    ${CONTAINER_CMD} images "${IMAGE_NAME}:${IMAGE_TAG}" --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}"
else
    echo ""
    echo "=========================================="
    echo "✗ Container build failed!"
    echo "=========================================="
    exit 1
fi

