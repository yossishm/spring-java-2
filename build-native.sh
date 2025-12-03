#!/bin/bash

# Build script for GraalVM Native Image (OSS Community Edition)
# This script builds a native executable of the Spring Boot application

set -e

echo "=========================================="
echo "Building Native Image with GraalVM OSS"
echo "=========================================="

# Check if GraalVM is installed
if ! command -v native-image &> /dev/null; then
    echo "ERROR: GraalVM native-image is not installed."
    echo "Please install GraalVM Community Edition:"
    echo "  - Download from: https://www.graalvm.org/downloads/"
    echo "  - Or use SDKMAN: sdk install java 21.0.2-graal"
    echo ""
    echo "Alternatively, use Docker build: ./build-native-docker.sh"
    exit 1
fi

# Check Java version
JAVA_VERSION=$(java -version 2>&1 | head -n 1 | cut -d'"' -f2 | sed '/^1\./s///' | cut -d'.' -f1)
if [ "$JAVA_VERSION" != "21" ]; then
    echo "WARNING: Java version is $JAVA_VERSION, expected 21"
    echo "Make sure you're using GraalVM JDK 21"
fi

echo "Using Java: $(java -version 2>&1 | head -n 1)"
echo "Using native-image: $(native-image --version 2>&1 | head -n 1)"
echo ""

# Clean previous builds
echo "Cleaning previous builds..."
./mvnw clean

# Build native image
echo "Building native image (this may take several minutes)..."
./mvnw package -Pnative -DskipTests

# Check if build was successful
if [ -f "target/gs-spring-boot-docker" ]; then
    echo ""
    echo "=========================================="
    echo "✓ Native image built successfully!"
    echo "=========================================="
    echo "Executable location: target/gs-spring-boot-docker"
    echo "File size: $(du -h target/gs-spring-boot-docker | cut -f1)"
    echo ""
    echo "To run the native executable:"
    echo "  ./target/gs-spring-boot-docker"
    echo ""
    echo "To build Docker image:"
    echo "  docker build -f Dockerfile.native -t spring-boot-native:latest ."
else
    echo ""
    echo "=========================================="
    echo "✗ Native image build failed!"
    echo "=========================================="
    echo "Check the build logs above for errors."
    exit 1
fi

