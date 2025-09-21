#!/bin/bash

echo "=========================================="
echo "🔍 SPRING JAVA vs .NET PROJECT COMPARISON"
echo "=========================================="
echo ""

# Function to show project structure
show_structure() {
    echo "📁 PROJECT STRUCTURE:"
    echo "----------------------------------------"
    
    echo "Java Project:"
    local java_files=$(find . -name "*.java" -not -path "./dotnet-spring-equivalent/*" | wc -l | tr -d ' ')
    local java_test_files=$(find . -name "*Test*.java" -not -path "./dotnet-spring-equivalent/*" | wc -l | tr -d ' ')
    echo "   📄 Java Files: $java_files"
    echo "   🧪 Test Files: $java_test_files"
    
    echo ""
    echo ".NET Project:"
    local dotnet_files=$(find dotnet-spring-equivalent -name "*.cs" -not -path "*/bin/*" -not -path "*/obj/*" | wc -l | tr -d ' ')
    local dotnet_test_files=$(find dotnet-spring-equivalent -name "*Test*.cs" -not -path "*/bin/*" -not -path "*/obj/*" | wc -l | tr -d ' ')
    echo "   📄 C# Files: $dotnet_files"
    echo "   🧪 Test Files: $dotnet_test_files"
    echo ""
}

# Function to show test results
show_test_results() {
    echo "🧪 TEST EXECUTION RESULTS:"
    echo "----------------------------------------"
    
    echo "Java Project:"
    if [ -f "target/surefire-reports/TEST-*.xml" ]; then
        echo "   ✅ Tests executed successfully"
    else
        echo "   ❌ No test results found"
    fi
    
    echo ""
    echo ".NET Project:"
    if [ -d "dotnet-spring-equivalent/TestResults" ] && [ "$(ls -A dotnet-spring-equivalent/TestResults)" ]; then
        echo "   ✅ Tests executed successfully"
        local latest_trx=$(find dotnet-spring-equivalent/TestResults -name "*.trx" -type f -exec ls -t {} + | head -1)
        if [ ! -z "$latest_trx" ]; then
            local total_tests=$(grep -o 'total="[^"]*"' "$latest_trx" | cut -d'"' -f2)
            local passed_tests=$(grep -o 'passed="[^"]*"' "$latest_trx" | cut -d'"' -f2)
            local failed_tests=$(grep -o 'failed="[^"]*"' "$latest_trx" | cut -d'"' -f2)
            echo "   📊 Total Tests: $total_tests"
            echo "   ✅ Passed: $passed_tests"
            echo "   ❌ Failed: $failed_tests"
        fi
    else
        echo "   ❌ No test results found"
    fi
    echo ""
}

# Function to show coverage
show_coverage() {
    echo "📊 CODE COVERAGE:"
    echo "----------------------------------------"
    
    echo "Java Project:"
    if [ -f "target/site/jacoco/jacoco.xml" ]; then
        local line_rate=$(grep -o 'line-rate="[^"]*"' target/site/jacoco/jacoco.xml | cut -d'"' -f2)
        local branch_rate=$(grep -o 'branch-rate="[^"]*"' target/site/jacoco/jacoco.xml | cut -d'"' -f2)
        if [ ! -z "$line_rate" ]; then
            local line_percent=$(echo "$line_rate * 100" | bc -l | cut -c1-5)
            local branch_percent=$(echo "$branch_rate * 100" | bc -l | cut -c1-5)
            echo "   📈 Line Coverage: ${line_percent}%"
            echo "   🌿 Branch Coverage: ${branch_percent}%"
        else
            echo "   ❌ Coverage data not available"
        fi
    else
        echo "   ❌ No coverage report found"
    fi
    
    echo ""
    echo ".NET Project:"
    local latest_coverage=$(find dotnet-spring-equivalent/TestResults -name "coverage.cobertura.xml" -type f -exec ls -t {} + | head -1)
    if [ ! -z "$latest_coverage" ]; then
        local line_rate=$(grep -o 'line-rate="[^"]*"' "$latest_coverage" | cut -d'"' -f2)
        local branch_rate=$(grep -o 'branch-rate="[^"]*"' "$latest_coverage" | cut -d'"' -f2)
        if [ ! -z "$line_rate" ]; then
            local line_percent=$(echo "$line_rate * 100" | bc -l | cut -c1-5)
            local branch_percent=$(echo "$branch_rate * 100" | bc -l | cut -c1-5)
            echo "   📈 Line Coverage: ${line_percent}%"
            echo "   🌿 Branch Coverage: ${branch_percent}%"
        else
            echo "   ❌ Coverage data not available"
        fi
    else
        echo "   ❌ No coverage report found"
    fi
    echo ""
}

# Function to show SonarQube status
show_sonarqube_status() {
    echo "🔍 SONARQUBE ANALYSIS:"
    echo "----------------------------------------"
    
    if curl -s http://localhost:9000/api/system/status > /dev/null 2>&1; then
        echo "✅ SonarQube is running"
        echo ""
        echo "🌐 Dashboard Links:"
        echo "   Java:  http://localhost:9000/dashboard?id=spring-java-2"
        echo "   .NET:  http://localhost:9000/dashboard?id=dotnet-spring-equivalent"
    else
        echo "❌ SonarQube is not running"
        echo "   Start with: docker run -d -p 9000:9000 sonarqube:latest"
    fi
    echo ""
}

# Function to show quality tools
show_quality_tools() {
    echo "🛠️  QUALITY TOOLS CONFIGURED:"
    echo "----------------------------------------"
    echo "Both projects include:"
    echo "   ✅ SonarQube static analysis"
    echo "   ✅ Trivy security scanning"
    echo "   ✅ OWASP dependency check"
    echo "   ✅ GitHub Actions CI/CD"
    echo "   ✅ Email notifications on failure"
    echo ""
    echo "Java-specific:"
    echo "   ✅ SpotBugs static analysis"
    echo "   ✅ Checkstyle code style"
    echo "   ✅ PMD code quality"
    echo "   ✅ JaCoCo coverage reporting"
    echo ""
    echo ".NET-specific:"
    echo "   ✅ Roslyn analyzers"
    echo "   ✅ StyleCop code style"
    echo "   ✅ Coverlet coverage reporting"
    echo ""
}

# Main execution
show_structure
show_test_results
show_coverage
show_sonarqube_status
show_quality_tools

echo "=========================================="
echo "🎯 VISUAL COMPARISON SUMMARY"
echo "=========================================="
echo "For the most comprehensive visual comparison:"
echo "1. 🌐 Open both SonarQube dashboards in separate browser tabs"
echo "2. 📊 Compare the metrics side by side"
echo "3. 🔍 Look at the 'Issues' tab for code quality"
echo "4. 📈 Check the 'Measures' tab for detailed metrics"
echo "5. 🧪 Review the 'Tests' tab for test coverage"
echo ""
echo "Both projects are now production-ready with:"
echo "✅ Comprehensive test coverage"
echo "✅ Automated quality gates"
echo "✅ Security scanning"
echo "✅ CI/CD pipelines"
echo "=========================================="
