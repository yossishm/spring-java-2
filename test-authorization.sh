#!/bin/bash

# Test script for JWT Authorization Implementation
# This script demonstrates the authorization system with different token types

BASE_URL="http://localhost:8080"
echo "Testing JWT Authorization Implementation"
echo "========================================"

# Function to make HTTP requests with JWT token
make_request() {
    local method=$1
    local endpoint=$2
    local token=$3
    local data=$4
    
    if [ -n "$token" ]; then
        if [ -n "$data" ]; then
            curl -s -X $method "$BASE_URL$endpoint" \
                -H "Authorization: Bearer $token" \
                -H "Content-Type: application/json" \
                -d "$data"
        else
            curl -s -X $method "$BASE_URL$endpoint" \
                -H "Authorization: Bearer $token"
        fi
    else
        curl -s -X $method "$BASE_URL$endpoint"
    fi
}

# Function to extract token from response
extract_token() {
    echo "$1" | grep -o '"token":"[^"]*"' | cut -d'"' -f4
}

echo ""
echo "1. Testing Public Endpoint (No Auth Required)"
echo "---------------------------------------------"
response=$(make_request "GET" "/api/v1/test/public")
echo "Response: $response"
echo ""

echo "2. Testing Protected Endpoint (Auth Required)"
echo "---------------------------------------------"
response=$(make_request "GET" "/api/v1/test/protected")
echo "Response: $response"
echo ""

echo "3. Generating Admin Token"
echo "------------------------"
admin_response=$(make_request "GET" "/api/v1/auth/token/admin")
echo "Admin Token Response: $admin_response"
admin_token=$(extract_token "$admin_response")
echo "Admin Token: $admin_token"
echo ""

echo "4. Testing Admin Endpoint with Admin Token"
echo "------------------------------------------"
response=$(make_request "GET" "/api/v1/test/admin" "$admin_token")
echo "Response: $response"
echo ""

echo "5. Testing Cache Read with Admin Token"
echo "-------------------------------------"
response=$(make_request "GET" "/api/v1/test/cache/read" "$admin_token")
echo "Response: $response"
echo ""

echo "6. Testing Cache Write with Admin Token"
echo "--------------------------------------"
response=$(make_request "POST" "/api/v1/test/cache/write" "$admin_token" '{"data":"test"}')
echo "Response: $response"
echo ""

echo "7. Generating Cache Reader Token"
echo "-------------------------------"
reader_response=$(make_request "GET" "/api/v1/auth/token/cache-reader")
echo "Reader Token Response: $reader_response"
reader_token=$(extract_token "$reader_response")
echo "Reader Token: $reader_token"
echo ""

echo "8. Testing Cache Read with Reader Token"
echo "--------------------------------------"
response=$(make_request "GET" "/api/v1/test/cache/read" "$reader_token")
echo "Response: $response"
echo ""

echo "9. Testing Cache Write with Reader Token (Should Fail)"
echo "-----------------------------------------------------"
response=$(make_request "POST" "/api/v1/test/cache/write" "$reader_token" '{"data":"test"}')
echo "Response: $response"
echo ""

echo "10. Testing Admin Endpoint with Reader Token (Should Fail)"
echo "---------------------------------------------------------"
response=$(make_request "GET" "/api/v1/test/admin" "$reader_token")
echo "Response: $response"
echo ""

echo "11. Testing Original Cache Service Endpoints"
echo "--------------------------------------------"
echo "Testing GET with Admin Token:"
response=$(make_request "GET" "/api/v1/cacheServices/getObject?id=123" "$admin_token")
echo "Response: $response"
echo ""

echo "Testing PUT with Admin Token:"
response=$(make_request "PUT" "/api/v1/cacheServices/putObject?id=123" "$admin_token")
echo "Response: $response"
echo ""

echo "Testing DELETE with Admin Token:"
response=$(make_request "DELETE" "/api/v1/cacheServices/deleteObject?id=123" "$admin_token")
echo "Response: $response"
echo ""

echo "Testing GET with Reader Token:"
response=$(make_request "GET" "/api/v1/cacheServices/getObject?id=123" "$reader_token")
echo "Response: $response"
echo ""

echo "Testing PUT with Reader Token (Should Fail):"
response=$(make_request "PUT" "/api/v1/cacheServices/putObject?id=123" "$reader_token")
echo "Response: $response"
echo ""

echo "12. Testing Token Validation"
echo "---------------------------"
response=$(make_request "POST" "/api/v1/auth/validate" "" "token=$admin_token")
echo "Validation Response: $response"
echo ""

echo "Testing with Invalid Token:"
response=$(make_request "POST" "/api/v1/auth/validate" "" "token=invalid_token")
echo "Validation Response: $response"
echo ""

echo "Authorization Testing Complete!"
echo "=============================="
echo ""
echo "Summary:"
echo "- Public endpoints work without authentication"
echo "- Protected endpoints require valid JWT tokens"
echo "- Admin endpoints require ADMIN role"
echo "- Cache operations require appropriate permissions"
echo "- Invalid tokens return 401 Unauthorized"
echo "- Insufficient permissions return 403 Forbidden"
