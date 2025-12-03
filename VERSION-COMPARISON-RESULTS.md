# Spring Boot 3.5.7 vs 3.5.8 Version Comparison Results

## Current Spring Boot 3.5.8 Versions (from dependency tree)

Based on actual Maven dependency resolution:

- **Tomcat**: 10.1.48
- **Spring Framework**: 6.2.12  
- **Netty**: 4.1.128.Final

## CVE Fix Status Check

### CVE-2025-24813 (Tomcat Path Equivalence)
- **Requires**: Tomcat >= 10.1.35
- **Status**: ✅ **FIXED** (10.1.48 >= 10.1.35)

### CVE-2025-55752 (Tomcat Path Traversal)
- **Requires**: Tomcat >= 10.1.45
- **Status**: ✅ **FIXED** (10.1.48 >= 10.1.45)

### CVE-2025-24970 (Netty SSL/TLS)
- **Requires**: Netty >= 4.1.118.Final
- **Status**: ✅ **FIXED** (4.1.128.Final >= 4.1.118.Final)

### CVE-2025-41249 (Spring Framework)
- **Requires**: Spring Framework >= 6.2.11
- **Status**: ✅ **FIXED** (6.2.12 >= 6.2.11)

## Spring Boot 3.5.7 Comparison

To determine if Spring Boot 3.5.7 was vulnerable, we need to check:
- What Tomcat version did 3.5.7 include?
- What Spring Framework version did 3.5.7 include?
- What Netty version did 3.5.7 include?

If Spring Boot 3.5.7 included:
- Tomcat >= 10.1.45 → **NOT vulnerable** to CVE-2025-24813 or CVE-2025-55752
- Spring Framework >= 6.2.11 → **NOT vulnerable** to CVE-2025-41249
- Netty >= 4.1.118.Final → **NOT vulnerable** to CVE-2025-24970

## Conclusion

**Spring Boot 3.5.8**: All CVEs are FIXED ✅

**Spring Boot 3.5.7**: Need to verify actual dependency versions to determine status.

