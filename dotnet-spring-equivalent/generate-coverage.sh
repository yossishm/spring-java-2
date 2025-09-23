#!/bin/bash

# Script to generate .NET coverage reports with normalized paths for Docker compatibility
# This addresses path mismatch issues between macOS local development and Docker containers

set -e

echo "🔍 Generating .NET Coverage Reports for Docker Compatibility"
echo "==========================================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Ensure we're in the project directory
cd "$(dirname "$0")"

# Clean previous results
echo -e "\n${BLUE}🧹 Cleaning previous test results...${NC}"
rm -rf TestResults
mkdir -p TestResults

# Restore dependencies
echo -e "\n${BLUE}📦 Restoring dependencies...${NC}"
dotnet restore

# Build the project
echo -e "\n${BLUE}🔨 Building project...${NC}"
dotnet build --configuration Release --no-restore

# Run tests with coverage using XPlat Code Coverage (generates cobertura format)
echo -e "\n${BLUE}🧪 Running tests with coverage collection...${NC}"
if dotnet test SpringJavaEquivalent.Tests/SpringJavaEquivalent.Tests.csproj --no-build --logger trx --results-directory ./TestResults --collect:"XPlat Code Coverage"; then
    echo -e "${GREEN}✅ Tests completed successfully${NC}"
else
    echo -e "${YELLOW}⚠️  Some tests failed, but continuing with analysis${NC}"
fi

# Find and copy the latest coverage file to a consistent location
echo -e "\n${BLUE}🔍 Locating coverage file...${NC}"
COVERAGE_FILE=$(find ./TestResults -name "coverage.cobertura.xml" 2>/dev/null | sort | tail -1)
if [ -n "$COVERAGE_FILE" ]; then
    cp "$COVERAGE_FILE" ./TestResults/coverage.cobertura.xml
    echo -e "${GREEN}✅ Coverage file found and copied: $COVERAGE_FILE${NC}"
else
    echo -e "${RED}❌ No coverage file found${NC}"
fi

# Check if coverage file was generated
if [ ! -f "./TestResults/coverage.cobertura.xml" ]; then
    echo -e "${RED}❌ Coverage file was not generated${NC}"
    exit 1
fi

echo -e "\n${BLUE}📊 Coverage report generated: ./TestResults/coverage.cobertura.xml${NC}"

# Show coverage summary
echo -e "\n${BLUE}📈 Coverage Summary:${NC}"
if command -v xmllint >/dev/null 2>&1; then
    # Extract coverage info using xmllint if available
    COVERAGE_RATE=$(xmllint --xpath "string(//coverage/@line-rate)" ./TestResults/coverage.cobertura.xml 2>/dev/null || echo "unknown")
    LINES_COVERED=$(xmllint --xpath "string(//coverage/@lines-covered)" ./TestResults/coverage.cobertura.xml 2>/dev/null || echo "unknown")
    LINES_VALID=$(xmllint --xpath "string(//coverage/@lines-valid)" ./TestResults/coverage.cobertura.xml 2>/dev/null || echo "unknown")

    echo "Line Coverage: $(printf "%.1f%%" "$(echo "$COVERAGE_RATE * 100" | bc -l 2>/dev/null || echo "0")")"
    echo "Lines Covered: $LINES_COVERED / $LINES_VALID"
else
    # Fallback: extract from XML manually
    COVERAGE_INFO=$(grep -o 'line-rate="[^"]*"' ./TestResults/coverage.cobertura.xml | head -1)
    echo "Coverage info: $COVERAGE_INFO"
fi

# Normalize paths for Docker compatibility (optional - uncomment if needed)
# This converts absolute macOS paths to relative paths
echo -e "\n${BLUE}🔧 Normalizing paths for Docker compatibility...${NC}"

# Option 1: Use reportgenerator to create relative paths
if command -v reportgenerator >/dev/null 2>&1; then
    echo -e "${YELLOW}Using reportgenerator to normalize paths...${NC}"
    reportgenerator -reports:./TestResults/coverage.cobertura.xml -targetdir:./TestResults/coverage-report -reporttypes:Cobertura -sourcedirs:.
    if [ -f "./TestResults/coverage-report/Cobertura.xml" ]; then
        mv ./TestResults/coverage-report/Cobertura.xml ./TestResults/coverage.cobertura.xml
        echo -e "${GREEN}✅ Paths normalized using reportgenerator${NC}"
    fi
else
    echo -e "${YELLOW}reportgenerator not available, paths may need manual adjustment${NC}"
fi

# Option 2: Manual path adjustment (uncomment if needed for Docker)
# PROJECT_ROOT="/Users/$(whoami)/dev/spring-java-2/dotnet-spring-equivalent"
# DOCKER_WORKSPACE="/app"
# sed -i '' "s|${PROJECT_ROOT}|${DOCKER_WORKSPACE}|g" ./TestResults/coverage.cobertura.xml

echo -e "\n${GREEN}✅ Coverage generation complete!${NC}"
echo -e "${BLUE}Coverage file: ./TestResults/coverage.cobertura.xml${NC}"
echo -e "${BLUE}Ready for SonarQube analysis${NC}"
