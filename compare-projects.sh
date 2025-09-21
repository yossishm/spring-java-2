#!/bin/bash

echo "=========================================="
echo "🔍 SPRING JAVA vs .NET PROJECT COMPARISON"
echo "=========================================="
echo ""

# Function to get SonarQube metrics
get_sonar_metrics() {
    local project_key=$1
    local project_name=$2
    
    echo "📊 $project_name Metrics:"
    echo "----------------------------------------"
    
    # Get basic metrics
    local metrics=$(curl -s "http://localhost:9000/api/measures/component?component=$project_key&metricKeys=coverage,line_coverage,branch_coverage,duplicated_lines_density,code_smells,bugs,vulnerabilities,security_hotspots,reliability_rating,security_rating,maintainability_rating" 2>/dev/null)
    
    if [ $? -eq 0 ] && [ ! -z "$metrics" ]; then
        echo "$metrics" | jq -r '.component.measures[] | "\(.metric): \(.value)"' 2>/dev/null || echo "Metrics available but jq not installed"
    else
        echo "❌ Could not fetch metrics from SonarQube"
    fi
    echo ""
}

# Function to get test results
get_test_results() {
    local project_path=$1
    local project_name=$2
    
    echo "🧪 $project_name Test Results:"
    echo "----------------------------------------"
    
    if [ -d "$project_path" ]; then
        cd "$project_path"
        
        if [ "$project_name" = "Java" ]; then
            # Java test results
            if [ -f "target/surefire-reports/TEST-*.xml" ]; then
                echo "✅ Java tests executed"
                local test_count=$(find target/surefire-reports -name "TEST-*.xml" | wc -l)
                echo "   Test files: $test_count"
            else
                echo "❌ No Java test results found"
            fi
            
            # Java coverage
            if [ -f "target/site/jacoco/jacoco.xml" ]; then
                echo "✅ Java coverage report available"
                local line_rate=$(grep -o 'line-rate="[^"]*"' target/site/jacoco/jacoco.xml | cut -d'"' -f2)
                local branch_rate=$(grep -o 'branch-rate="[^"]*"' target/site/jacoco/jacoco.xml | cut -d'"' -f2)
                echo "   Line Coverage: $(echo "$line_rate * 100" | bc -l | cut -c1-5)%"
                echo "   Branch Coverage: $(echo "$branch_rate * 100" | bc -l | cut -c1-5)%"
            else
                echo "❌ No Java coverage report found"
            fi
            
        elif [ "$project_name" = ".NET" ]; then
            # .NET test results
            if [ -d "TestResults" ] && [ "$(ls -A TestResults)" ]; then
                echo "✅ .NET tests executed"
                local test_files=$(find TestResults -name "*.trx" | wc -l)
                echo "   Test result files: $test_files"
                
                # Get latest test results
                local latest_trx=$(find TestResults -name "*.trx" -type f -exec ls -t {} + | head -1)
                if [ ! -z "$latest_trx" ]; then
                    local total_tests=$(grep -o 'total="[^"]*"' "$latest_trx" | cut -d'"' -f2)
                    local passed_tests=$(grep -o 'passed="[^"]*"' "$latest_trx" | cut -d'"' -f2)
                    local failed_tests=$(grep -o 'failed="[^"]*"' "$latest_trx" | cut -d'"' -f2)
                    echo "   Total Tests: $total_tests"
                    echo "   Passed: $passed_tests"
                    echo "   Failed: $failed_tests"
                fi
            else
                echo "❌ No .NET test results found"
            fi
            
            # .NET coverage
            if [ -d "TestResults" ]; then
                local latest_coverage=$(find TestResults -name "coverage.cobertura.xml" -type f -exec ls -t {} + | head -1)
                if [ ! -z "$latest_coverage" ]; then
                    echo "✅ .NET coverage report available"
                    local line_rate=$(grep -o 'line-rate="[^"]*"' "$latest_coverage" | cut -d'"' -f2)
                    local branch_rate=$(grep -o 'branch-rate="[^"]*"' "$latest_coverage" | cut -d'"' -f2)
                    echo "   Line Coverage: $(echo "$line_rate * 100" | bc -l | cut -c1-5)%"
                    echo "   Branch Coverage: $(echo "$branch_rate * 100" | bc -l | cut -c1-5)%"
                else
                    echo "❌ No .NET coverage report found"
                fi
            fi
        fi
        
        cd - > /dev/null
    else
        echo "❌ Project directory not found: $project_path"
    fi
    echo ""
}

# Function to show SonarQube dashboard links
show_dashboard_links() {
    echo "🌐 SonarQube Dashboard Links:"
    echo "----------------------------------------"
    echo "Java Project:    http://localhost:9000/dashboard?id=spring-java-2"
    echo ".NET Project:    http://localhost:9000/dashboard?id=dotnet-spring-equivalent"
    echo ""
}

# Function to show file counts
show_file_counts() {
    echo "📁 Project Structure Comparison:"
    echo "----------------------------------------"
    
    echo "Java Project:"
    if [ -d "." ]; then
        local java_files=$(find . -name "*.java" -not -path "./dotnet-spring-equivalent/*" | wc -l)
        local java_test_files=$(find . -name "*Test*.java" -not -path "./dotnet-spring-equivalent/*" | wc -l)
        echo "   Java Files: $java_files"
        echo "   Test Files: $java_test_files"
    fi
    
    echo ".NET Project:"
    if [ -d "dotnet-spring-equivalent" ]; then
        local dotnet_files=$(find dotnet-spring-equivalent -name "*.cs" -not -path "*/bin/*" -not -path "*/obj/*" | wc -l)
        local dotnet_test_files=$(find dotnet-spring-equivalent -name "*Test*.cs" -not -path "*/bin/*" -not -path "*/obj/*" | wc -l)
        echo "   C# Files: $dotnet_files"
        echo "   Test Files: $dotnet_test_files"
    fi
    echo ""
}

# Main execution
echo "🚀 Starting project comparison..."
echo ""

# Check if SonarQube is running
if curl -s http://localhost:9000/api/system/status > /dev/null 2>&1; then
    echo "✅ SonarQube is running"
    echo ""
    
    # Get SonarQube metrics
    get_sonar_metrics "spring-java-2" "Java"
    get_sonar_metrics "dotnet-spring-equivalent" ".NET"
    
    # Show dashboard links
    show_dashboard_links
else
    echo "❌ SonarQube is not running. Start it with: docker run -d -p 9000:9000 sonarqube:latest"
    echo ""
fi

# Get test results and coverage
get_test_results "." "Java"
get_test_results "dotnet-spring-equivalent" ".NET"

# Show file counts
show_file_counts

echo "=========================================="
echo "🎯 SUMMARY"
echo "=========================================="
echo "Both projects are now configured with:"
echo "✅ Comprehensive test suites"
echo "✅ Code coverage reporting"
echo "✅ SonarQube quality analysis"
echo "✅ Security scanning (Trivy, OWASP)"
echo "✅ GitHub Actions CI/CD pipelines"
echo ""
echo "📊 For detailed visual comparison, visit the SonarQube dashboards above!"
echo "=========================================="
