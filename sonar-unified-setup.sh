#!/bin/bash

# Unified SonarQube Setup and Analysis Script
# This script sets up SonarQube and runs analysis for both Java and .NET projects

set -e

echo "🔍 Setting up Unified SonarQube Analysis (Java + .NET)..."
echo "======================================================"

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
    
    # Run SonarQube container
    docker run -d --name sonarqube -p 9000:9000 -p 9092:9092 sonarqube:latest
    
    echo -e "${GREEN}✅ SonarQube container started. Waiting for it to be ready...${NC}"
    # Wait for SonarQube to be ready (adjust sleep time if needed)
    sleep 30
    
    SONARQUBE_URL="http://localhost:9000"
    echo -e "${GREEN}✅ SonarQube is accessible at: ${SONARQUBE_URL}${NC}"
    echo "   Default credentials: admin/admin"
fi

echo -e "\n${BLUE}📊 Preparing Java Project for SonarQube Analysis...${NC}"

# Generate Java coverage and test reports
echo -e "${YELLOW}1. Running Java tests with coverage...${NC}"
mvn clean test jacoco:report

echo -e "\n${BLUE}📊 Preparing .NET Project for SonarQube Analysis...${NC}"

# Generate .NET coverage and test reports
echo -e "${YELLOW}2. Running .NET tests with coverage...${NC}"
cd dotnet-spring-equivalent
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --collect:"XPlat Code Coverage" --results-directory ../TestResults

# Install and run report generator for .NET
echo -e "${YELLOW}3. Generating .NET coverage report...${NC}"
dotnet tool install -g dotnet-reportgenerator-globaltool --version 5.2.0
reportgenerator -reports:"../TestResults/**/coverage.cobertura.xml" -targetdir:"../TestResults/CoverageReport" -reporttypes:"Html;Cobertura;JsonSummary"
cd ..

echo -e "\n${BLUE}🔍 Running Unified SonarQube Analysis...${NC}"

# Run SonarQube analysis with unified configuration
echo -e "${YELLOW}4. Starting SonarQube analysis for both projects...${NC}"
mvn sonar:sonar -Dsonar.host.url=${SONARQUBE_URL} -Dsonar.project.properties=sonar-project-unified.properties

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Unified SonarQube analysis completed successfully${NC}"
    echo -e "   View unified report at: ${SONARQUBE_URL}${NC}"
    echo -e "\n${BLUE}📊 What you'll see in SonarQube:${NC}"
    echo -e "   🌐 Unified dashboard with both Java and .NET metrics"
    echo -e "   📈 Combined coverage reports (Java JaCoCo + .NET Cobertura)"
    echo -e "   🔍 Code quality analysis for both languages"
    echo -e "   🚨 Security vulnerabilities across both projects"
    echo -e "   📋 Technical debt and maintainability ratings"
    echo -e "   🎯 Quality gate status for the entire codebase"
else
    echo -e "${RED}❌ SonarQube analysis failed${NC}"
    exit 1
fi

echo -e "\n${BLUE}✨ Unified SonarQube Setup and Analysis Complete!${NC}"
echo "======================================================"

echo -e "\n${BLUE}🎯 Key Benefits of Unified Analysis:${NC}"
echo -e "   📊 Single dashboard for both Java and .NET"
echo -e "   📈 Combined coverage metrics and trends"
echo -e "   🔍 Cross-language code quality insights"
echo -e "   🚨 Unified security vulnerability tracking"
echo -e "   📋 Overall project health assessment"
echo -e "   🎯 Single quality gate for entire codebase"

echo -e "\n${BLUE}📱 Access your unified report:${NC}"
echo -e "   🌐 URL: ${SONARQUBE_URL}"
echo -e "   👤 Username: admin"
echo -e "   🔑 Password: admin"
