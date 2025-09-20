#!/bin/bash

# Local SonarQube Analysis Script (No Authentication Required)
# This script runs SonarQube analysis locally without requiring authentication

set -e

echo "🔍 Running Local SonarQube Analysis..."
echo "====================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check if SonarQube is running
if ! curl -s http://localhost:9000 > /dev/null; then
    echo -e "${RED}❌ SonarQube is not running. Please start it first with:${NC}"
    echo -e "${YELLOW}   docker run -d --name sonarqube -p 9000:9000 sonarqube:latest${NC}"
    exit 1
fi

echo -e "${GREEN}✅ SonarQube is running at http://localhost:9000${NC}"

echo -e "\n${BLUE}📊 Running Java Analysis...${NC}"

# Run Java analysis with local SonarQube (no auth required for local)
mvn sonar:sonar -Dsonar.host.url=http://localhost:9000 -Dsonar.login=admin -Dsonar.password=admin

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Java SonarQube analysis completed successfully${NC}"
else
    echo -e "${YELLOW}⚠️  Java analysis failed, but continuing with .NET...${NC}"
fi

echo -e "\n${BLUE}📊 Running .NET Analysis...${NC}"

# Run .NET analysis
cd dotnet-spring-equivalent

# Install SonarQube scanner for .NET
dotnet tool install -g dotnet-sonarscanner --version 5.15.1

# Begin .NET analysis
dotnet sonarscanner begin /k:"dotnet-spring-equivalent" /d:sonar.host.url=http://localhost:9000 /d:sonar.login=admin /d:sonar.password=admin

# Build .NET project
dotnet build --configuration Release

# End .NET analysis
dotnet sonarscanner end /d:sonar.login=admin /d:sonar.password=admin

cd ..

echo -e "\n${GREEN}✅ SonarQube Analysis Complete!${NC}"
echo "====================================="

echo -e "\n${BLUE}🌐 Access your SonarQube dashboard:${NC}"
echo -e "   URL: http://localhost:9000"
echo -e "   Username: admin"
echo -e "   Password: admin"

echo -e "\n${BLUE}📊 What you'll see:${NC}"
echo -e "   📈 Java project analysis with coverage"
echo -e "   📈 .NET project analysis with coverage"
echo -e "   🔍 Code quality metrics for both languages"
echo -e "   🚨 Security vulnerabilities"
echo -e "   📋 Technical debt analysis"
echo -e "   🎯 Quality gate status"

echo -e "\n${BLUE}💡 Pro Tips:${NC}"
echo -e "   • Use the 'Projects' tab to switch between Java and .NET"
echo -e "   • Check 'Code' tab for detailed file-by-file analysis"
echo -e "   • Review 'Issues' tab for code quality problems"
echo -e "   • Monitor 'Security' tab for vulnerabilities"
