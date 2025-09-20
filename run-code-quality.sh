#!/bin/bash

# Code Quality Analysis Script for Spring Java Project
# This script runs all code quality checks and generates comprehensive reports

set -e

echo "🔍 Starting Code Quality Analysis..."
echo "=================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Create reports directory
mkdir -p reports

echo -e "${BLUE}📊 Running Code Quality Checks...${NC}"

# 1. Compile and run tests with coverage
echo -e "${YELLOW}1. Running Tests with JaCoCo Coverage...${NC}"
mvn clean test jacoco:report
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Tests and coverage completed successfully${NC}"
else
    echo -e "${RED}❌ Tests failed${NC}"
    exit 1
fi

# 2. Run Checkstyle
echo -e "${YELLOW}2. Running Checkstyle Analysis...${NC}"
mvn checkstyle:check
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Checkstyle passed${NC}"
else
    echo -e "${RED}❌ Checkstyle violations found${NC}"
fi

# 3. Run PMD
echo -e "${YELLOW}3. Running PMD Analysis...${NC}"
mvn pmd:check
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ PMD passed${NC}"
else
    echo -e "${RED}❌ PMD violations found${NC}"
fi

# 4. Run SpotBugs
echo -e "${YELLOW}4. Running SpotBugs Analysis...${NC}"
mvn spotbugs:check
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ SpotBugs passed${NC}"
else
    echo -e "${RED}❌ SpotBugs issues found${NC}"
fi

# 5. Generate SpotBugs HTML report
echo -e "${YELLOW}5. Generating SpotBugs HTML Report...${NC}"
mvn spotbugs:gui &
SPOTBUGS_PID=$!
sleep 2
kill $SPOTBUGS_PID 2>/dev/null || true

# 6. Run Mutation Testing (Pitest)
echo -e "${YELLOW}6. Running Mutation Testing...${NC}"
mvn org.pitest:pitest-maven:mutationCoverage
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Mutation testing completed${NC}"
else
    echo -e "${RED}❌ Mutation testing failed${NC}"
fi

# 7. Copy reports to reports directory
echo -e "${YELLOW}7. Collecting Reports...${NC}"
cp -r target/site/jacoco reports/ 2>/dev/null || true
cp -r target/site/checkstyle reports/ 2>/dev/null || true
cp -r target/site/pmd reports/ 2>/dev/null || true
cp -r target/spotbugs reports/ 2>/dev/null || true
cp -r target/pit-reports reports/ 2>/dev/null || true

# 8. Generate summary report
echo -e "${YELLOW}8. Generating Summary Report...${NC}"
cat > reports/quality-summary.md << EOF
# Code Quality Analysis Summary

Generated on: $(date)

## Reports Generated

### 1. Code Coverage (JaCoCo)
- **Location**: reports/jacoco/index.html
- **Command**: \`mvn jacoco:report\`

### 2. Code Style (Checkstyle)
- **Location**: reports/checkstyle/index.html
- **Command**: \`mvn checkstyle:check\`

### 3. Code Quality (PMD)
- **Location**: reports/pmd/index.html
- **Command**: \`mvn pmd:check\`

### 4. Bug Detection (SpotBugs)
- **Location**: reports/spotbugs/
- **Command**: \`mvn spotbugs:check\`

### 5. Mutation Testing (Pitest)
- **Location**: reports/pit-reports/
- **Command**: \`mvn org.pitest:pitest-maven:mutationCoverage\`

## Quick Commands

\`\`\`bash
# Run all quality checks
./run-code-quality.sh

# Run individual checks
mvn checkstyle:check
mvn pmd:check
mvn spotbugs:check
mvn jacoco:report

# View reports
open reports/jacoco/index.html
open reports/checkstyle/index.html
open reports/pmd/index.html
\`\`\`

## SonarQube Integration

To run SonarQube analysis:

\`\`\`bash
# Start SonarQube server (if not running)
docker run -d --name sonarqube -p 9000:9000 sonarqube:latest

# Run SonarQube analysis
mvn sonar:sonar -Dsonar.host.url=http://localhost:9000
\`\`\`

Then open http://localhost:9000 to view the results.
EOF

echo -e "${GREEN}🎉 Code Quality Analysis Complete!${NC}"
echo -e "${BLUE}📁 Reports available in: reports/${NC}"
echo -e "${BLUE}📋 Summary: reports/quality-summary.md${NC}"

# Open reports in browser (macOS)
if command -v open &> /dev/null; then
    echo -e "${YELLOW}Opening coverage report in browser...${NC}"
    open reports/jacoco/index.html 2>/dev/null || true
fi

echo ""
echo -e "${GREEN}✅ All quality checks completed!${NC}"
echo -e "${BLUE}Check the reports/ directory for detailed results.${NC}"
