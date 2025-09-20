#!/bin/bash

# .NET Reports Viewer Script
# This script helps you view and analyze all generated .NET reports

set -e

echo "📊 .NET Reports Viewer"
echo "====================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to check if file exists and show info
check_report() {
    local file_path="$1"
    local description="$2"
    local icon="$3"
    
    if [ -f "$file_path" ]; then
        local size=$(du -h "$file_path" | cut -f1)
        echo -e "${GREEN}✅ $icon $description${NC}"
        echo -e "   📁 Path: $file_path"
        echo -e "   📏 Size: $size"
        echo -e "   📅 Modified: $(stat -f "%Sm" "$file_path" 2>/dev/null || stat -c "%y" "$file_path" 2>/dev/null || echo "Unknown")"
        return 0
    else
        echo -e "${RED}❌ $icon $description${NC}"
        echo -e "   📁 Path: $file_path (not found)"
        return 1
    fi
}

# Function to show file content preview
preview_file() {
    local file_path="$1"
    local max_lines="${2:-10}"
    
    if [ -f "$file_path" ]; then
        echo -e "\n${BLUE}📄 Preview (first $max_lines lines):${NC}"
        echo "----------------------------------------"
        head -n "$max_lines" "$file_path"
        local total_lines=$(wc -l < "$file_path")
        if [ "$total_lines" -gt "$max_lines" ]; then
            echo "... ($((total_lines - max_lines)) more lines)"
        fi
        echo "----------------------------------------"
    fi
}

echo -e "${BLUE}🔍 Checking for available reports...${NC}\n"

# Check coverage reports
echo -e "${YELLOW}📊 COVERAGE REPORTS:${NC}"
check_report "TestResults/CoverageReport/index.html" "HTML Coverage Report" "🌐"
check_report "TestResults/CoverageReport/Cobertura.xml" "Cobertura XML Coverage" "📄"
check_report "TestResults/CoverageReport/Summary.json" "Coverage Summary JSON" "📋"

# Check security reports
echo -e "\n${YELLOW}🔒 SECURITY REPORTS:${NC}"
check_report "reports/dotnet/security-audit.txt" "Security Audit (Text)" "📝"
check_report "reports/dotnet/security-audit.json" "Security Audit (JSON)" "📄"
check_report "reports/dotnet/trivy-security-scan.txt" "Trivy Security Scan" "🔍"
check_report "reports/dotnet/trivy-results.sarif" "SARIF Security Report" "📊"

# Check code quality reports
echo -e "\n${YELLOW}📋 CODE QUALITY REPORTS:${NC}"
check_report "reports/dotnet/code-analysis.txt" "Code Analysis Report" "🔍"
check_report "reports/dotnet/code-style.txt" "Code Style Report" "🎨"
check_report "reports/dotnet/dependencies.txt" "Dependencies Report" "📦"

echo -e "\n${BLUE}🎯 Available Actions:${NC}"
echo "1. View coverage report in browser"
echo "2. Show security audit summary"
echo "3. Show Trivy scan results"
echo "4. Show code analysis issues"
echo "5. Show code style issues"
echo "6. Show dependencies"
echo "7. Open all reports in browser"
echo "8. Generate missing reports"
echo "9. Exit"

while true; do
    echo -e "\n${YELLOW}Choose an action (1-9):${NC}"
    read -r choice
    
    case $choice in
        1)
            if [ -f "TestResults/CoverageReport/index.html" ]; then
                echo -e "${GREEN}🌐 Opening coverage report in browser...${NC}"
                open "TestResults/CoverageReport/index.html" 2>/dev/null || xdg-open "TestResults/CoverageReport/index.html" 2>/dev/null || echo "Please open TestResults/CoverageReport/index.html manually"
            else
                echo -e "${RED}❌ Coverage report not found. Run generate-dotnet-reports.sh first.${NC}"
            fi
            ;;
        2)
            if [ -f "reports/dotnet/security-audit.txt" ]; then
                preview_file "reports/dotnet/security-audit.txt" 20
            else
                echo -e "${RED}❌ Security audit report not found.${NC}"
            fi
            ;;
        3)
            if [ -f "reports/dotnet/trivy-security-scan.txt" ]; then
                preview_file "reports/dotnet/trivy-security-scan.txt" 30
            else
                echo -e "${RED}❌ Trivy scan report not found.${NC}"
            fi
            ;;
        4)
            if [ -f "reports/dotnet/code-analysis.txt" ]; then
                preview_file "reports/dotnet/code-analysis.txt" 25
            else
                echo -e "${RED}❌ Code analysis report not found.${NC}"
            fi
            ;;
        5)
            if [ -f "reports/dotnet/code-style.txt" ]; then
                preview_file "reports/dotnet/code-style.txt" 25
            else
                echo -e "${RED}❌ Code style report not found.${NC}"
            fi
            ;;
        6)
            if [ -f "reports/dotnet/dependencies.txt" ]; then
                preview_file "reports/dotnet/dependencies.txt" 30
            else
                echo -e "${RED}❌ Dependencies report not found.${NC}"
            fi
            ;;
        7)
            echo -e "${GREEN}🌐 Opening all available reports...${NC}"
            if [ -f "TestResults/CoverageReport/index.html" ]; then
                open "TestResults/CoverageReport/index.html" 2>/dev/null || xdg-open "TestResults/CoverageReport/index.html" 2>/dev/null || echo "Please open TestResults/CoverageReport/index.html manually"
            fi
            # Open reports directory
            open "reports/dotnet" 2>/dev/null || xdg-open "reports/dotnet" 2>/dev/null || echo "Please open reports/dotnet directory manually"
            ;;
        8)
            echo -e "${GREEN}🔧 Generating missing reports...${NC}"
            ./generate-dotnet-reports.sh
            ;;
        9)
            echo -e "${GREEN}👋 Goodbye!${NC}"
            exit 0
            ;;
        *)
            echo -e "${RED}❌ Invalid choice. Please enter 1-9.${NC}"
            ;;
    esac
done
