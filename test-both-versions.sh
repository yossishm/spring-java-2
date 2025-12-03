#!/bin/bash

echo "=========================================="
echo "Spring Boot 3.5.7 vs 3.5.8 Comparison"
echo "=========================================="
echo ""

# Backup pom.xml
cp pom.xml pom.xml.backup

# Test 3.5.7
echo "=== Testing Spring Boot 3.5.7 ==="
sed -i.bak 's/3.5.8/3.5.7/g' pom.xml
mvn clean dependency:resolve -q > /dev/null 2>&1

echo "Tomcat:"
mvn dependency:tree -Dincludes=org.apache.tomcat.embed:tomcat-embed-core -q 2>&1 | grep "tomcat-embed-core" | head -1 | sed 's/.*:jar:\([^:]*\):.*/\1/'

echo "Spring Framework:"
mvn dependency:tree -Dincludes=org.springframework:spring-core -q 2>&1 | grep "spring-core" | head -1 | sed 's/.*:jar:\([^:]*\):.*/\1/'

echo "Netty:"
mvn dependency:tree -Dincludes=io.netty:netty-codec-http -q 2>&1 | grep "netty-codec-http" | head -1 | sed 's/.*:jar:\([^:]*\):.*/\1/'

echo ""
echo "=== Testing Spring Boot 3.5.8 ==="
sed -i.bak 's/3.5.7/3.5.8/g' pom.xml
mvn clean dependency:resolve -q > /dev/null 2>&1

echo "Tomcat:"
mvn dependency:tree -Dincludes=org.apache.tomcat.embed:tomcat-embed-core -q 2>&1 | grep "tomcat-embed-core" | head -1 | sed 's/.*:jar:\([^:]*\):.*/\1/'

echo "Spring Framework:"
mvn dependency:tree -Dincludes=org.springframework:spring-core -q 2>&1 | grep "spring-core" | head -1 | sed 's/.*:jar:\([^:]*\):.*/\1/'

echo "Netty:"
mvn dependency:tree -Dincludes=io.netty:netty-codec-http -q 2>&1 | grep "netty-codec-http" | head -1 | sed 's/.*:jar:\([^:]*\):.*/\1/'

# Restore
mv pom.xml.backup pom.xml
rm -f pom.xml.bak

echo ""
echo "=== CVE Fix Requirements ==="
echo "CVE-2025-24813: Tomcat >= 10.1.35"
echo "CVE-2025-55752: Tomcat >= 10.1.45"
echo "CVE-2025-24970: Netty >= 4.1.118.Final"
echo "CVE-2025-41249: Spring Framework >= 6.2.11"
