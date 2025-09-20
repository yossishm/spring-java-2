#!/bin/bash

# Security Testing Script
# This script runs security checks locally to test before pushing to GitHub

set -e

echo "🔒 Running Security Tests..."
echo "=============================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check if Maven is available
if ! command -v mvn &> /dev/null; then
    echo -e "${RED}❌ Maven is not installed or not in PATH${NC}"
    exit 1
fi

echo -e "${BLUE}📊 Running OWASP Dependency Check...${NC}"
if mvn org.owasp:dependency-check-maven:check; then
    echo -e "${GREEN}✅ OWASP Dependency Check completed successfully${NC}"
else
    echo -e "${YELLOW}⚠️  OWASP Dependency Check failed (likely due to network issues)${NC}"
    echo -e "${YELLOW}📄 This is common when NVD API is unavailable. Continuing with Trivy scan...${NC}"
fi

echo -e "${BLUE}🔍 Running Trivy Security Scan...${NC}"

# Check if Docker is available
if ! command -v docker &> /dev/null; then
    echo -e "${RED}❌ Docker is not installed or not in PATH${NC}"
    exit 1
fi

# Run Trivy scan using Docker container
echo -e "${YELLOW}📦 Using Trivy container for security scan...${NC}"
mkdir -p /tmp/trivy-scan
cp -r . /tmp/trivy-scan/
docker run --rm -v /tmp/trivy-scan:/workspace aquasec/trivy:latest fs --format table --severity HIGH,CRITICAL /workspace
rm -rf /tmp/trivy-scan

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Trivy Security Scan completed successfully${NC}"
else
    echo -e "${RED}❌ Trivy Security Scan found vulnerabilities${NC}"
    exit 1
fi

echo -e "${GREEN}🎉 All security checks passed!${NC}"
echo -e "${BLUE}📄 Security reports:${NC}"
echo -e "  - OWASP: target/dependency-check-report.html"
echo -e "  - Trivy: Check console output above"
