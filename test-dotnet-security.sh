#!/bin/bash

# .NET Security Testing Script
# This script runs security checks locally for the .NET project

set -e

echo "🔒 Running .NET Security Tests..."
echo "=================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check if .NET is available
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ .NET is not installed or not in PATH${NC}"
    exit 1
fi

# Check if Docker is available
if ! command -v docker &> /dev/null; then
    echo -e "${RED}❌ Docker is not installed or not in PATH${NC}"
    exit 1
fi

echo -e "${BLUE}📊 Running .NET Security Audit...${NC}"
cd dotnet-spring-equivalent

if dotnet list package --vulnerable --include-transitive; then
    echo -e "${GREEN}✅ .NET Security Audit completed successfully${NC}"
else
    echo -e "${YELLOW}⚠️  .NET Security Audit found vulnerabilities${NC}"
    echo -e "${YELLOW}📄 Check the output above for details${NC}"
fi

echo -e "${BLUE}🔍 Running Trivy Security Scan for .NET...${NC}"

# Build the Alpine-based image first
echo -e "${YELLOW}📦 Building .NET Alpine image for security scan...${NC}"
docker build -f Dockerfile.alpine -t dotnet-app:alpine .

# Run Trivy scan using Docker container on the new image
echo -e "${YELLOW}📦 Using Trivy container for .NET security scan...${NC}"
docker run --rm -v /var/run/docker.sock:/var/run/docker.sock aquasec/trivy:latest image --format table --severity HIGH,CRITICAL dotnet-app:alpine

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Trivy Security Scan completed successfully${NC}"
else
    echo -e "${RED}❌ Trivy Security Scan found vulnerabilities${NC}"
    exit 1
fi

echo -e "${GREEN}🎉 All .NET security checks passed!${NC}"
echo -e "${BLUE}📄 Security reports:${NC}"
echo -e "  - .NET Security Audit: Check console output above"
echo -e "  - Trivy: Check console output above"
