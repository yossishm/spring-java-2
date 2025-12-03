#!/bin/bash

echo "=========================================="
echo "Spring Boot Version Comparison"
echo "=========================================="
echo ""

echo "Current Spring Boot 3.5.8 dependency versions:"
echo ""
mvn dependency:tree -Dincludes=org.apache.tomcat.embed:tomcat-embed-core,io.netty:netty-codec-http,org.springframework:spring-core 2>&1 | \
grep -E "(tomcat-embed-core|netty-codec-http|spring-core)" | \
sed 's/.*:jar:\([^:]*\):.*/\1/' | \
awk '{
    if ($0 ~ /tomcat/) print "  Tomcat: " $0
    else if ($0 ~ /netty/) print "  Netty: " $0  
    else if ($0 ~ /spring-core/) print "  Spring Framework: " $0
}'

echo ""
echo "CVE Fix Requirements:"
echo "  CVE-2025-24813: Tomcat >= 10.1.35"
echo "  CVE-2025-55752: Tomcat >= 10.1.45"
echo "  CVE-2025-24970: Netty >= 4.1.118.Final"
echo "  CVE-2025-41249: Spring Framework >= 6.2.11"
echo ""
