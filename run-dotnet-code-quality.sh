#!/bin/bash

# .NET Code Quality Analysis Script
# This script runs all code quality checks for the .NET project

set -e

echo "🔍 Starting .NET Code Quality Analysis..."
echo "========================================"

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

# Create reports directory
mkdir -p reports

echo -e "${BLUE}📊 Running .NET Code Quality Checks...${NC}"

# Change to .NET project directory
cd dotnet-spring-equivalent

# 1. Restore dependencies
echo -e "${YELLOW}1. Restoring Dependencies...${NC}\n"
dotnet restore
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Dependencies restored successfully${NC}"
else
    echo -e "${RED}❌ Dependency restoration failed${NC}"
    exit 1
fi

# 2. Build project
echo -e "\n${YELLOW}2. Building Project...${NC}\n"
dotnet build --configuration Release --no-restore
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Build completed successfully${NC}"
else
    echo -e "${RED}❌ Build failed${NC}"
    exit 1
fi

# 3. Run tests with coverage
echo -e "\n${YELLOW}3. Running Tests with Coverage...${NC}\n"
dotnet test --configuration Release --no-build --verbosity normal --collect:"XPlat Code Coverage" --results-directory ../TestResults
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Tests completed successfully${NC}"
else
    echo -e "${RED}❌ Tests failed${NC}"
    exit 1
fi

# 4. Generate coverage report
echo -e "\n${YELLOW}4. Generating Coverage Report...${NC}\n"
dotnet tool install -g dotnet-reportgenerator-globaltool --version 5.2.0
reportgenerator -reports:"../TestResults/**/coverage.cobertura.xml" -targetdir:"../TestResults/CoverageReport" -reporttypes:"Html;Cobertura"
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Coverage report generated successfully${NC}"
else
    echo -e "${YELLOW}⚠️  Coverage report generation failed${NC}"
fi

# 5. Run security audit
echo -e "\n${YELLOW}5. Running Security Audit...${NC}\n"
dotnet list package --vulnerable --include-transitive
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Security audit completed successfully${NC}"
else
    echo -e "${YELLOW}⚠️  Security audit found vulnerabilities${NC}"
fi

# 6. Run code analysis
echo -e "\n${YELLOW}6. Running Code Analysis...${NC}\n"
dotnet build --configuration Release --verbosity normal /p:RunAnalyzersDuringBuild=true /p:EnableNETAnalyzers=true
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Code analysis completed successfully${NC}"
else
    echo -e "${YELLOW}⚠️  Code analysis found issues${NC}"
fi

# 7. Check code style
echo -e "\n${YELLOW}7. Checking Code Style...${NC}\n"
dotnet format --verify-no-changes --verbosity diagnostic
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Code style check passed${NC}"
else
    echo -e "${YELLOW}⚠️  Code style check found issues${NC}"
fi

# 8. Run Trivy security scan
echo -e "\n${YELLOW}8. Running Trivy Security Scan...${NC}\n"
if command -v docker &> /dev/null; then
    mkdir -p /tmp/dotnet-trivy-scan
    cp -r . /tmp/dotnet-trivy-scan/
    docker run --rm -v /tmp/dotnet-trivy-scan:/workspace aquasec/trivy:latest fs --format table --severity HIGH,CRITICAL /workspace
    rm -rf /tmp/dotnet-trivy-scan
    echo -e "${GREEN}✅ Trivy security scan completed${NC}"
else
    echo -e "${YELLOW}⚠️  Docker not available, skipping Trivy scan${NC}"
fi

echo -e "\n${BLUE}✨ .NET Code Quality Analysis Complete!${NC}"
echo "========================================"

echo -e "\n${BLUE}📊 Reports are available in:${NC}"
echo "   - Test Results: TestResults/"
echo "   - Coverage Report: TestResults/CoverageReport/index.html"
echo "   - Security Audit: Check console output above"
echo "   - Trivy Scan: Check console output above"

echo -e "\n${BLUE}🚀 To run SonarQube analysis, configure SONAR_TOKEN and run:${NC}"
echo "   dotnet tool install -g dotnet-sonarscanner"
echo "   dotnet sonarscanner begin /k:\"dotnet-spring-equivalent\" /d:sonar.host.url=\$SONAR_HOST_URL /d:sonar.login=\$SONAR_TOKEN"
echo "   dotnet build --configuration Release"
echo "   dotnet sonarscanner end /d:sonar.login=\$SONAR_TOKEN"
