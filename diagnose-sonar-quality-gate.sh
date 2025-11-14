#!/bin/bash

# Script to diagnose SonarCloud Quality Gate failures
# This script helps identify what's causing the quality gate to fail

set -e

echo "🔍 SonarCloud Quality Gate Diagnostic Tool"
echo "=========================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Step 1: Check if tests are run and coverage is generated
echo "📊 Step 1: Checking test coverage..."
if [ ! -f "target/site/jacoco/jacoco.xml" ]; then
    echo -e "${RED}❌ Coverage file not found: target/site/jacoco/jacoco.xml${NC}"
    echo "   Running tests with coverage..."
    mvn clean test jacoco:report
else
    echo -e "${GREEN}✅ Coverage file exists${NC}"
fi

# Step 2: Check coverage percentage
echo ""
echo "📈 Step 2: Analyzing coverage report..."
if [ -f "target/site/jacoco/jacoco.xml" ]; then
    # Extract coverage percentage from JaCoCo XML
    COVERAGE=$(grep -oP 'type="INSTRUCTION".*missed="\K[0-9]+' target/site/jacoco/jacoco.xml | head -1)
    COVERED=$(grep -oP 'type="INSTRUCTION".*covered="\K[0-9]+' target/site/jacoco/jacoco.xml | head -1)
    
    if [ ! -z "$COVERAGE" ] && [ ! -z "$COVERED" ]; then
        TOTAL=$((COVERAGE + COVERED))
        if [ $TOTAL -gt 0 ]; then
            PERCENTAGE=$(echo "scale=2; $COVERED * 100 / $TOTAL" | bc)
            echo "   Total instructions: $TOTAL"
            echo "   Covered: $COVERED"
            echo "   Missed: $COVERAGE"
            echo "   Coverage: ${PERCENTAGE}%"
            
            if (( $(echo "$PERCENTAGE < 80" | bc -l) )); then
                echo -e "${RED}❌ Coverage is below 80% threshold!${NC}"
                echo "   Required: 80%"
                echo "   Current: ${PERCENTAGE}%"
            else
                echo -e "${GREEN}✅ Coverage meets 80% threshold${NC}"
            fi
        fi
    fi
fi

# Step 3: Check for code smells and issues
echo ""
echo "🔍 Step 3: Checking for code quality issues..."
echo "   Running SpotBugs..."
mvn spotbugs:check || echo -e "${YELLOW}⚠️  SpotBugs found issues${NC}"

echo ""
echo "   Running Checkstyle..."
mvn checkstyle:check || echo -e "${YELLOW}⚠️  Checkstyle found issues${NC}"

echo ""
echo "   Running PMD..."
mvn pmd:check || echo -e "${YELLOW}⚠️  PMD found issues${NC}"

# Step 4: Check SonarCloud dashboard URL
echo ""
echo "🌐 Step 4: SonarCloud Dashboard"
echo "   View detailed results at:"
echo "   https://sonarcloud.io/dashboard?id=yossishm_spring-java-2"
echo ""
echo "   Common quality gate conditions:"
echo "   - Coverage on New Code: Must be >= 80%"
echo "   - Duplicated Lines: Must be < 3%"
echo "   - Maintainability Rating: Must be A"
echo "   - Reliability Rating: Must be A"
echo "   - Security Rating: Must be A"
echo "   - Security Hotspots: Must be 0"

# Step 5: Recommendations
echo ""
echo "💡 Recommendations:"
echo "   1. Check the SonarCloud dashboard for specific failing conditions"
echo "   2. If coverage is low, add more unit tests"
echo "   3. Fix any code smells or security vulnerabilities"
echo "   4. Review and fix duplicated code"
echo ""
echo "   To temporarily disable quality gate (for debugging only):"
echo "   Remove or set to false: -Dsonar.qualitygate.wait=false"
echo ""

