# Spring Boot 3.5.7 vs 3.5.8 - Comparison Results

## Summary

Based on Maven dependency resolution:

### Spring Boot 3.5.7
- Tomcat: (checking...)
- Spring Framework: 6.2.12
- Netty: (checking...)

### Spring Boot 3.5.8  
- Tomcat: 10.1.48
- Spring Framework: 6.2.12
- Netty: 4.1.128.Final

## Key Finding

**Spring Framework version is the SAME (6.2.12) in both 3.5.7 and 3.5.8**

This means:
- ✅ CVE-2025-41249 was already FIXED in Spring Boot 3.5.7 (Spring Framework 6.2.12 >= 6.2.11)

## CVE Status

### CVE-2025-41249 (Spring Framework)
- **Status in 3.5.7**: ✅ FIXED (Spring Framework 6.2.12 >= 6.2.11)
- **Status in 3.5.8**: ✅ FIXED (Spring Framework 6.2.12 >= 6.2.11)
- **Conclusion**: Both versions are NOT vulnerable

### Other CVEs
- Need to check Tomcat and Netty versions in 3.5.7 to determine status
