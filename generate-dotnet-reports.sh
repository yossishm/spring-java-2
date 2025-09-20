#!/bin/bash

# .NET Reports Generation Script
# This script generates all quality and security reports for the .NET project

set -e

echo "📊 Generating .NET Quality & Security Reports..."
echo "=============================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Create reports directory
mkdir -p reports/dotnet
cd dotnet-spring-equivalent

echo -e "${BLUE}🔧 Setting up .NET project...${NC}"

# Restore and build
dotnet restore
dotnet build --configuration Release --no-restore

echo -e "${BLUE}🧪 Running tests with coverage...${NC}"

# Run tests with coverage
dotnet test --configuration Release --no-build --verbosity normal --collect:"XPlat Code Coverage" --results-directory ../TestResults

echo -e "${BLUE}📈 Generating coverage report...${NC}"

# Install and run report generator
dotnet tool install -g dotnet-reportgenerator-globaltool --version 5.2.0
reportgenerator -reports:"../TestResults/**/coverage.cobertura.xml" -targetdir:"../TestResults/CoverageReport" -reporttypes:"Html;Cobertura;JsonSummary"

echo -e "${BLUE}🔒 Running security audit...${NC}"

# Generate security audit report
dotnet list package --vulnerable --include-transitive --format json > ../reports/dotnet/security-audit.json
dotnet list package --vulnerable --include-transitive > ../reports/dotnet/security-audit.txt

echo -e "${BLUE}🔍 Running Trivy security scan...${NC}"

# Run Trivy and generate SARIF report
mkdir -p /tmp/dotnet-trivy-scan
cp -r . /tmp/dotnet-trivy-scan/
docker run --rm -v /tmp/dotnet-trivy-scan:/workspace aquasec/trivy:latest fs --format table --severity HIGH,CRITICAL /workspace > ../reports/dotnet/trivy-security-scan.txt
docker run --rm -v /tmp/dotnet-trivy-scan:/workspace aquasec/trivy:latest fs --format sarif --output /workspace/trivy-results.sarif /workspace
cp /tmp/dotnet-trivy-scan/trivy-results.sarif ../reports/dotnet/
rm -rf /tmp/dotnet-trivy-scan

echo -e "${BLUE}📋 Generating code analysis report...${NC}"

# Generate code analysis report
dotnet build --configuration Release --verbosity normal /p:RunAnalyzersDuringBuild=true /p:EnableNETAnalyzers=true > ../reports/dotnet/code-analysis.txt 2>&1 || true

echo -e "${BLUE}🎨 Generating code style report...${NC}"

# Generate code style report
dotnet format --verify-no-changes --verbosity diagnostic > ../reports/dotnet/code-style.txt 2>&1 || true

echo -e "${BLUE}📊 Generating project dependencies report...${NC}"

# Generate dependencies report
dotnet list package --include-transitive > ../reports/dotnet/dependencies.txt

echo -e "${GREEN}✅ All reports generated successfully!${NC}"
echo "=============================================="

echo -e "\n${BLUE}📁 Report Locations:${NC}"
echo "   📊 Coverage Report:     TestResults/CoverageReport/index.html"
echo "   🔒 Security Audit:      reports/dotnet/security-audit.txt"
echo "   🔍 Trivy Scan:          reports/dotnet/trivy-security-scan.txt"
echo "   📋 Code Analysis:       reports/dotnet/code-analysis.txt"
echo "   🎨 Code Style:          reports/dotnet/code-style.txt"
echo "   📦 Dependencies:        reports/dotnet/dependencies.txt"
echo "   📄 SARIF Report:        reports/dotnet/trivy-results.sarif"

echo -e "\n${BLUE}🌐 To view coverage report in browser:${NC}"
echo "   open TestResults/CoverageReport/index.html"

echo -e "\n${BLUE}📊 Report Summary:${NC}"
if [ -f "../TestResults/CoverageReport/Summary.json" ]; then
    echo "   Coverage Summary:"
    cat ../TestResults/CoverageReport/Summary.json | jq -r '.summary | "   - Line Coverage: \(.linecoverage)%\n   - Branch Coverage: \(.branchcoverage)%\n   - Method Coverage: \(.methodcoverage)%"' 2>/dev/null || echo "   - Coverage data available in HTML report"
fi

echo -e "\n${BLUE}🔒 Security Summary:${NC}"
if [ -f "../reports/dotnet/security-audit.json" ]; then
    VULNERABILITIES=$(jq -r '.vulnerabilities | length' ../reports/dotnet/security-audit.json 2>/dev/null || echo "0")
    echo "   - Vulnerable Packages: $VULNERABILITIES"
fi

echo -e "\n${GREEN}🎉 Report generation complete!${NC}"
