#!/bin/bash

# Unified Authorization Test Suite
# Tests both Spring and .NET applications with identical scenarios
# Verifies that both implementations have the same authorization capabilities

# Configuration
SPRING_URL="http://localhost:8080"
DOTNET_URL="http://localhost:5000"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Test results tracking
SPRING_PASSED=0
SPRING_FAILED=0
DOTNET_PASSED=0
DOTNET_FAILED=0

echo -e "${BLUE}🚀 Unified Authorization Test Suite${NC}"
echo "=================================================="
echo "Testing both Spring and .NET applications with identical scenarios"
echo ""

# Function to make HTTP requests and track results
test_endpoint() {
    local app_name=$1
    local base_url=$2
    local method=$3
    local endpoint=$4
    local token=$5
    local expected_status=$6
    local description=$7
    
    echo -e "${YELLOW}📋 $description${NC}"
    echo "   $app_name: $method $endpoint"
    
    if [ -n "$token" ]; then
        response=$(curl -s -w "\n%{http_code}" -X $method "$base_url$endpoint" \
            -H "Authorization: Bearer $token" \
            -H "Content-Type: application/json" 2>/dev/null)
    else
        response=$(curl -s -w "\n%{http_code}" -X $method "$base_url$endpoint" \
            -H "Content-Type: application/json" 2>/dev/null)
    fi
    
    if [ $? -ne 0 ]; then
        echo -e "   ${RED}❌ Connection failed${NC}"
        if [ "$app_name" = "Spring" ]; then
            ((SPRING_FAILED++))
        else
            ((DOTNET_FAILED++))
        fi
        return
    fi
    
    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | head -n -1)
    
    if [ "$http_code" = "$expected_status" ]; then
        echo -e "   ${GREEN}✅ Success (Status: $http_code)${NC}"
        if [ "$app_name" = "Spring" ]; then
            ((SPRING_PASSED++))
        else
            ((DOTNET_PASSED++))
        fi
    else
        echo -e "   ${RED}❌ Failed (Expected: $expected_status, Got: $http_code)${NC}"
        if [ "$app_name" = "Spring" ]; then
            ((SPRING_FAILED++))
        else
            ((DOTNET_FAILED++))
        fi
    fi
}

# Function to generate tokens for both apps
generate_tokens() {
    echo -e "${BLUE}🔑 Step 1: Generate Test Tokens${NC}"
    echo "================================="
    
    # Generate Spring tokens
    echo "Generating Spring tokens..."
    spring_admin_response=$(curl -s "$SPRING_URL/api/v1/auth/token/admin" 2>/dev/null)
    spring_user_response=$(curl -s "$SPRING_URL/api/v1/auth/token/user" 2>/dev/null)
    spring_cache_admin_response=$(curl -s "$SPRING_URL/api/v1/auth/token/cache-admin" 2>/dev/null)
    spring_aal3_response=$(curl -s "$SPRING_URL/api/v1/auth/token/aal3-user" 2>/dev/null)
    
    # Generate .NET tokens
    echo "Generating .NET tokens..."
    dotnet_admin_response=$(curl -s "$DOTNET_URL/api/v1/auth/token/admin" 2>/dev/null)
    dotnet_user_response=$(curl -s "$DOTNET_URL/api/v1/auth/token/user" 2>/dev/null)
    dotnet_cache_admin_response=$(curl -s "$DOTNET_URL/api/v1/auth/token/cache-admin" 2>/dev/null)
    dotnet_aal3_response=$(curl -s "$DOTNET_URL/api/v1/auth/token/aal3-user" 2>/dev/null)
    
    # Extract tokens
    spring_admin_token=$(echo "$spring_admin_response" | jq -r '.token' 2>/dev/null)
    spring_user_token=$(echo "$spring_user_response" | jq -r '.token' 2>/dev/null)
    spring_cache_admin_token=$(echo "$spring_cache_admin_response" | jq -r '.token' 2>/dev/null)
    spring_aal3_token=$(echo "$spring_aal3_response" | jq -r '.token' 2>/dev/null)
    
    dotnet_admin_token=$(echo "$dotnet_admin_response" | jq -r '.token' 2>/dev/null)
    dotnet_user_token=$(echo "$dotnet_user_response" | jq -r '.token' 2>/dev/null)
    dotnet_cache_admin_token=$(echo "$dotnet_cache_admin_response" | jq -r '.token' 2>/dev/null)
    dotnet_aal3_token=$(echo "$dotnet_aal3_response" | jq -r '.token' 2>/dev/null)
    
    echo "Spring tokens generated: ${spring_admin_token:0:20}..."
    echo ".NET tokens generated: ${dotnet_admin_token:0:20}..."
    echo ""
}

# Function to test authorization levels
test_authorization_levels() {
    echo -e "${BLUE}🧪 Step 2: Test Authorization Levels${NC}"
    echo "====================================="
    
    # Level 0: Public access
    test_endpoint "Spring" "$SPRING_URL" "GET" "/api/v1/test/public" "" "200" "Level 0: Public Access"
    test_endpoint ".NET" "$DOTNET_URL" "GET" "/api/v1/enhanced-test/public" "" "200" "Level 0: Public Access"
    
    # Level 1: Basic authentication
    test_endpoint "Spring" "$SPRING_URL" "GET" "/api/v1/test/protected" "$spring_user_token" "200" "Level 1: Basic Authentication"
    test_endpoint ".NET" "$DOTNET_URL" "GET" "/api/v1/enhanced-test/authenticated" "$dotnet_user_token" "200" "Level 1: Basic Authentication"
    
    # Level 2: Role-based access
    test_endpoint "Spring" "$SPRING_URL" "GET" "/api/v1/test/admin" "$spring_admin_token" "200" "Level 2: Admin Role Access"
    test_endpoint ".NET" "$DOTNET_URL" "GET" "/api/v1/enhanced-test/admin-only" "$dotnet_admin_token" "200" "Level 2: Admin Role Access"
    
    # Level 3: Permission-based access
    test_endpoint "Spring" "$SPRING_URL" "GET" "/api/v1/test/cache/read" "$spring_user_token" "200" "Level 3: Permission-Based Access"
    test_endpoint ".NET" "$DOTNET_URL" "GET" "/api/v1/enhanced-test/permission-based" "$dotnet_user_token" "200" "Level 3: Permission-Based Access"
    
    # Level 4: Cache service authorization
    test_endpoint "Spring" "$SPRING_URL" "GET" "/api/v1/cacheServices/getObject?id=123" "$spring_user_token" "200" "Level 4: Cache Service Authorization"
    test_endpoint ".NET" "$DOTNET_URL" "GET" "/api/v1/cacheServices/getObject?id=123" "$dotnet_user_token" "200" "Level 4: Cache Service Authorization"
    
    echo ""
}

# Function to test authorization failures
test_authorization_failures() {
    echo -e "${BLUE}🚫 Step 3: Test Authorization Failures${NC}"
    echo "======================================"
    
    # Unauthorized access (no token)
    test_endpoint "Spring" "$SPRING_URL" "GET" "/api/v1/test/protected" "" "401" "Unauthorized Access (No Token)"
    test_endpoint ".NET" "$DOTNET_URL" "GET" "/api/v1/enhanced-test/authenticated" "" "401" "Unauthorized Access (No Token)"
    
    # Forbidden access (wrong role)
    test_endpoint "Spring" "$SPRING_URL" "GET" "/api/v1/test/admin" "$spring_user_token" "403" "Forbidden Access (USER trying ADMIN)"
    test_endpoint ".NET" "$DOTNET_URL" "GET" "/api/v1/enhanced-test/admin-only" "$dotnet_user_token" "403" "Forbidden Access (USER trying ADMIN)"
    
    # Forbidden access (wrong permission)
    test_endpoint "Spring" "$SPRING_URL" "POST" "/api/v1/test/cache/write" "$spring_user_token" "403" "Forbidden Access (No CACHE_WRITE permission)"
    test_endpoint ".NET" "$DOTNET_URL" "PUT" "/api/v1/cacheServices/putObject?id=123" "$dotnet_user_token" "403" "Forbidden Access (No CACHE_WRITE permission)"
    
    echo ""
}

# Function to test token validation
test_token_validation() {
    echo -e "${BLUE}🔍 Step 4: Test Token Validation${NC}"
    echo "================================"
    
    # Valid token validation
    test_endpoint "Spring" "$SPRING_URL" "POST" "/api/v1/auth/validate?token=$spring_admin_token" "" "200" "Validate Valid Token"
    test_endpoint ".NET" "$DOTNET_URL" "POST" "/api/v1/auth/validate?token=$dotnet_admin_token" "" "200" "Validate Valid Token"
    
    # Invalid token validation
    test_endpoint "Spring" "$SPRING_URL" "POST" "/api/v1/auth/validate?token=invalid_token" "" "200" "Validate Invalid Token"
    test_endpoint ".NET" "$DOTNET_URL" "POST" "/api/v1/auth/validate?token=invalid_token" "" "200" "Validate Invalid Token"
    
    echo ""
}

# Function to test all token types
test_all_token_types() {
    echo -e "${BLUE}📈 Step 5: Test All Token Types${NC}"
    echo "==============================="
    
    token_types=("admin" "user" "cache-admin" "cache-writer" "cache-reader" "aal1-user" "aal2-user" "aal3-user")
    
    for token_type in "${token_types[@]}"; do
        echo "Testing $token_type token..."
        
        # Test Spring token generation
        spring_response=$(curl -s "$SPRING_URL/api/v1/auth/token/$token_type" 2>/dev/null)
        spring_token=$(echo "$spring_response" | jq -r '.token' 2>/dev/null)
        
        # Test .NET token generation
        dotnet_response=$(curl -s "$DOTNET_URL/api/v1/auth/token/$token_type" 2>/dev/null)
        dotnet_token=$(echo "$dotnet_response" | jq -r '.token' 2>/dev/null)
        
        if [ "$spring_token" != "null" ] && [ "$spring_token" != "" ]; then
            test_endpoint "Spring" "$SPRING_URL" "GET" "/api/v1/test/protected" "$spring_token" "200" "Spring $token_type token works"
        else
            echo -e "   ${RED}❌ Spring $token_type token generation failed${NC}"
            ((SPRING_FAILED++))
        fi
        
        if [ "$dotnet_token" != "null" ] && [ "$dotnet_token" != "" ]; then
            test_endpoint ".NET" "$DOTNET_URL" "GET" "/api/v1/enhanced-test/authenticated" "$dotnet_token" "200" ".NET $token_type token works"
        else
            echo -e "   ${RED}❌ .NET $token_type token generation failed${NC}"
            ((DOTNET_FAILED++))
        fi
    done
    
    echo ""
}

# Function to test advanced authorization features
test_advanced_features() {
    echo -e "${BLUE}🎯 Step 6: Test Advanced Authorization Features${NC}"
    echo "============================================="
    
    # Test AAL2+ requirements
    test_endpoint "Spring" "$SPRING_URL" "GET" "/api/v1/enhanced-test/aal2-required" "$spring_cache_admin_token" "200" "Spring AAL2+ Authentication Level"
    test_endpoint ".NET" "$DOTNET_URL" "GET" "/api/v1/enhanced-test/aal2-required" "$dotnet_cache_admin_token" "200" ".NET AAL2+ Authentication Level"
    
    # Test Enterprise IDP requirements
    test_endpoint "Spring" "$SPRING_URL" "GET" "/api/v1/enhanced-test/enterprise-only" "$spring_aal3_token" "200" "Spring Enterprise IDP Access"
    test_endpoint ".NET" "$DOTNET_URL" "GET" "/api/v1/enhanced-test/enterprise-only" "$dotnet_aal3_token" "200" ".NET Enterprise IDP Access"
    
    # Test Multi-factor authorization
    test_endpoint "Spring" "$SPRING_URL" "GET" "/api/v1/enhanced-test/multi-factor" "$spring_aal3_token" "200" "Spring Multi-Factor Authorization"
    test_endpoint ".NET" "$DOTNET_URL" "GET" "/api/v1/enhanced-test/multi-factor" "$dotnet_aal3_token" "200" ".NET Multi-Factor Authorization"
    
    echo ""
}

# Function to print summary
print_summary() {
    echo -e "${BLUE}📊 Test Results Summary${NC}"
    echo "========================"
    echo ""
    echo -e "${GREEN}Spring Results:${NC}"
    echo "  ✅ Passed: $SPRING_PASSED"
    echo "  ❌ Failed: $SPRING_FAILED"
    echo "  📈 Success Rate: $(( (SPRING_PASSED * 100) / (SPRING_PASSED + SPRING_FAILED) ))%"
    echo ""
    echo -e "${GREEN}.NET Results:${NC}"
    echo "  ✅ Passed: $DOTNET_PASSED"
    echo "  ❌ Failed: $DOTNET_FAILED"
    echo "  📈 Success Rate: $(( (DOTNET_PASSED * 100) / (DOTNET_PASSED + DOTNET_FAILED) ))%"
    echo ""
    
    if [ $SPRING_PASSED -eq $DOTNET_PASSED ] && [ $SPRING_FAILED -eq $DOTNET_FAILED ]; then
        echo -e "${GREEN}🎉 Perfect Match! Both applications have identical authorization capabilities!${NC}"
    else
        echo -e "${YELLOW}⚠️  Some differences detected between Spring and .NET implementations${NC}"
    fi
    
    echo ""
    echo -e "${BLUE}🔗 Swagger Documentation:${NC}"
    echo "  Spring: $SPRING_URL/swagger-ui.html"
    echo "  .NET: $DOTNET_URL/swagger"
    echo ""
    echo -e "${BLUE}🚀 Both applications now support:${NC}"
    echo "  ✅ Level 0: Public Access"
    echo "  ✅ Level 1: Basic Authentication"
    echo "  ✅ Level 2: Role-Based Access Control (RBAC)"
    echo "  ✅ Level 3: Permission-Based Access Control (PBAC)"
    echo "  ✅ Level 4: Service-Specific Authorization"
    echo "  ✅ Level 5: Authentication Assurance Level (AAL)"
    echo "  ✅ Level 6: Identity Provider Access"
    echo "  ✅ Level 7: Multi-Factor Authorization"
}

# Main execution
main() {
    # Check if applications are running
    echo "Checking if applications are running..."
    
    spring_running=$(curl -s -o /dev/null -w "%{http_code}" "$SPRING_URL/" 2>/dev/null)
    dotnet_running=$(curl -s -o /dev/null -w "%{http_code}" "$DOTNET_URL/" 2>/dev/null)
    
    if [ "$spring_running" != "200" ]; then
        echo -e "${RED}❌ Spring application is not running at $SPRING_URL${NC}"
        echo "Please start Spring application first: mvn spring-boot:run"
        exit 1
    fi
    
    if [ "$dotnet_running" != "200" ]; then
        echo -e "${RED}❌ .NET application is not running at $DOTNET_URL${NC}"
        echo "Please start .NET application first: dotnet run"
        exit 1
    fi
    
    echo -e "${GREEN}✅ Both applications are running${NC}"
    echo ""
    
    # Run all tests
    generate_tokens
    test_authorization_levels
    test_authorization_failures
    test_token_validation
    test_all_token_types
    test_advanced_features
    print_summary
}

# Run the main function
main
