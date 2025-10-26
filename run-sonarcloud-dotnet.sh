#!/bin/bash

# SonarCloud .NET Analysis Script
# This script runs SonarCloud analysis for the .NET project with proper configuration

set -e

echo "🔍 Running SonarCloud Analysis for .NET Project..."
echo "=================================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check if SONAR_TOKEN is set
if [[ -z "$SONAR_TOKEN" ]]; then
    echo -e "${RED}❌ SONAR_TOKEN environment variable is not set${NC}"
    echo -e "${YELLOW}Please set your SonarCloud token:${NC}"
    echo -e "   export SONAR_TOKEN=your_sonarcloud_token_here"
    exit 1
fi

# Check if .NET is available
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ .NET is not installed or not in PATH${NC}"
    exit 1
fi

# Install SonarScanner if not already installed
echo -e "${BLUE}📦 Installing/Updating SonarScanner for .NET...${NC}"
dotnet tool install -g dotnet-sonarscanner --version 5.7.1

# Change to .NET project directory
cd dotnet-spring-equivalent

echo -e "${BLUE}🔨 Step 1: Building project...${NC}"
dotnet build --configuration Release

echo -e "${BLUE}🧪 Step 2: Running tests with coverage...${NC}"
dotnet test --configuration Release --collect:"XPlat Code Coverage" --results-directory ./TestResults

echo -e "${BLUE}📊 Step 3: Starting SonarCloud analysis...${NC}"

# Begin SonarCloud analysis
dotnet sonarscanner begin \
  /k:"spring-java-2-dotnet" \
  /d:sonar.host.url="https://sonarcloud.io" \
  /d:sonar.login="$SONAR_TOKEN" \
  /d:sonar.projectBaseDir="." \
  /d:sonar.sources="." \
  /d:sonar.tests="SpringJavaEquivalent.Tests" \
  /d:sonar.cs.analyzer.projectOutPaths="bin/Release" \
  /d:sonar.cs.vstest.reportsPaths="TestResults/*.trx" \
  /d:sonar.cs.vscoveragexml.reportsPaths="TestResults/**/coverage.cobertura.xml" \
  /d:sonar.exclusions="**/bin/**,**/obj/**,**/TestResults/**" \
  /d:sonar.test.exclusions="**/bin/**,**/obj/**,**/TestResults/**" \
  /d:sonar.qualitygate.wait=true

# Build again for analysis
echo -e "${BLUE}🔨 Step 4: Building for analysis...${NC}"
dotnet build --configuration Release

# End SonarCloud analysis
echo -e "${BLUE}📊 Step 5: Ending SonarCloud analysis...${NC}"
dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"

cd ..

echo -e "\n${GREEN}✅ SonarCloud analysis completed!${NC}"
echo "=========================================="

echo -e "\n${BLUE}🌐 View your results:${NC}"
echo -e "   URL: https://sonarcloud.io/project/overview?id=spring-java-2-dotnet"
echo -e "   Organization: Check your SonarCloud dashboard"

echo -e "\n${BLUE}💡 If you still get errors:${NC}"
echo -e "   1. Verify the project key 'spring-java-2-dotnet' exists in SonarCloud"
echo -e "   2. Check your SONAR_TOKEN has proper permissions"
echo -e "   3. Ensure your organization is correctly configured"
