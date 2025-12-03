#!/bin/bash

echo "=========================================="
echo "Spring Boot 3.5.7 vs 3.5.8 Comparison"
echo "=========================================="
echo ""

# Save current version
CURRENT_VERSION=$(grep -A 2 "spring-boot-starter-parent" pom.xml | grep "version" | sed 's/.*<version>\(.*\)<\/version>.*/\1/')
echo "Current Spring Boot version in pom.xml: $CURRENT_VERSION"
echo ""

# Function to get dependency versions
get_versions() {
    local boot_version=$1
    echo "=== Spring Boot $boot_version ==="
    
    # Temporarily change version
    sed -i.bak "s/<version>$CURRENT_VERSION<\/version>/<version>$boot_version<\/version>/" pom.xml
    
    # Get versions - need to clean and resolve
    mvn clean dependency:resolve -q > /dev/null 2>&1
    
    echo "Tomcat version:"
    mvn dependency:tree -Dincludes=org.apache.tomcat.embed:tomcat-embed-core -q 2>&1 | \
    grep "tomcat-embed-core" | head -1 | sed 's/.*:jar:\([^:]*\):.*/\1/'
    
    echo "Netty version:"
    mvn dependency:tree -Dincludes=io.netty:netty-codec-http -q 2>&1 | \
    grep "netty-codec-http" | head -1 | sed 's/.*:jar:\([^:]*\):.*/\1/'
    
    echo "Spring Framework version:"
    mvn dependency:tree -Dincludes=org.springframework:spring-core -q 2>&1 | \
    grep "spring-core" | head -1 | sed 's/.*:jar:\([^:]*\):.*/\1/'
    
    echo ""
}

echo "Checking Spring Boot 3.5.7..."
get_versions "3.5.7"

echo "Checking Spring Boot 3.5.8..."
get_versions "3.5.8"

# Restore original version
mv pom.xml.bak pom.xml 2>/dev/null || sed -i.bak "s/<version>3.5.[78]<\/version>/<version>$CURRENT_VERSION<\/version>/" pom.xml
rm -f pom.xml.bak

echo "=========================================="
echo "CVE Fix Status Comparison"
echo "=========================================="
echo ""
echo "CVE-2025-24813: Fixed in Tomcat 10.1.35+"
echo "CVE-2025-55752: Fixed in Tomcat 10.1.45+"
echo "CVE-2025-24970: Fixed in Netty 4.1.118.Final+"
echo "CVE-2025-41249: Fixed in Spring Framework 6.2.11+"
echo ""
