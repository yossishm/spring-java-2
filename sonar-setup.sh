#!/bin/bash

# SonarQube Setup and Analysis Script
# This script sets up SonarQube locally and runs analysis

set -e

echo "🔍 Setting up SonarQube for Code Quality Analysis..."
echo "=================================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo -e "${RED}❌ Docker is not running. Please start Docker first.${NC}"
    exit 1
fi

# Check if SonarQube container is already running
if docker ps | grep -q sonarqube; then
    echo -e "${YELLOW}⚠️  SonarQube is already running${NC}"
    SONARQUBE_URL="http://localhost:9000"
else
    echo -e "${YELLOW}🐳 Starting SonarQube container...${NC}"
    
    # Stop and remove existing container if it exists
    docker stop sonarqube 2>/dev/null || true
    docker rm sonarqube 2>/dev/null || true
    
    # Start SonarQube
    docker run -d --name sonarqube \
        -p 9000:9000 \
        -e SONAR_ES_BOOTSTRAP_CHECKS_DISABLE=true \
        sonarqube:latest
    
    echo -e "${BLUE}⏳ Waiting for SonarQube to start (this may take a few minutes)...${NC}"
    
    # Wait for SonarQube to be ready
    for i in {1..60}; do
        if curl -s http://localhost:9000/api/system/status | grep -q '"status":"UP"'; then
            echo -e "${GREEN}✅ SonarQube is ready!${NC}"
            break
        fi
        echo -n "."
        sleep 5
    done
    
    if [ $i -eq 60 ]; then
        echo -e "${RED}❌ SonarQube failed to start within 5 minutes${NC}"
        exit 1
    fi
    
    SONARQUBE_URL="http://localhost:9000"
fi

echo -e "${BLUE}🌐 SonarQube is available at: ${SONARQUBE_URL}${NC}"
echo -e "${YELLOW}Default credentials: admin/admin${NC}"

# Run SonarQube analysis
echo -e "${YELLOW}🔍 Running SonarQube analysis...${NC}"
mvn clean compile test jacoco:report
mvn sonar:sonar \
    -Dsonar.host.url=$SONARQUBE_URL \
    -Dsonar.login=admin \
    -Dsonar.password=admin

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ SonarQube analysis completed successfully!${NC}"
    echo -e "${BLUE}📊 View results at: ${SONARQUBE_URL}${NC}"
    
    # Open browser (macOS)
    if command -v open &> /dev/null; then
        echo -e "${YELLOW}Opening SonarQube in browser...${NC}"
        open $SONARQUBE_URL
    fi
else
    echo -e "${RED}❌ SonarQube analysis failed${NC}"
    exit 1
fi

echo ""
echo -e "${GREEN}🎉 SonarQube setup and analysis complete!${NC}"
echo -e "${BLUE}💡 Tips:${NC}"
echo -e "   - Login with admin/admin"
echo -e "   - Check the 'Issues' tab for code quality problems"
echo -e "   - Review the 'Measures' tab for metrics and coverage"
echo -e "   - Use 'docker stop sonarqube' to stop the container"
