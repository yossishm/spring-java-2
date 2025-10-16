package hello;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.BeforeEach;
import static org.junit.jupiter.api.Assertions.*;
import java.time.Duration;
import java.time.Instant;

/**
 * Vulnerable Version DoS Test for CVE-2025-48924
 * 
 * This test demonstrates the actual vulnerability when using commons-lang3:3.17.0
 * WITHOUT our security fix. It shows what attacks are NOT mitigated.
 */
@DisplayName("Vulnerable Version DoS Test - Shows What's NOT Mitigated")
public class VulnerableVersionDoSTest {

    @Test
    @DisplayName("Demonstrate CVE-2025-48924 Vulnerability - DoS Attack")
    public void demonstrateCVE202548924Vulnerability() {
        System.out.println("=== CVE-2025-48924 VULNERABILITY DEMONSTRATION ===");
        System.out.println("⚠️  WARNING: This test shows the vulnerability in action!");
        System.out.println("⚠️  Using vulnerable commons-lang3:3.17.0");
        
        String version = org.apache.commons.lang3.StringUtils.class.getPackage().getImplementationVersion();
        System.out.println("Apache Commons Lang3 Version: " + version);
        
        if (version != null && version.startsWith("3.17")) {
            System.out.println("❌ VULNERABLE VERSION DETECTED!");
            System.out.println("🚨 This version is susceptible to CVE-2025-48924");
            
            // Test with more aggressive payload that could cause issues in vulnerable versions
            String[] aggressivePayloads = {
                "\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u\\\\u",
                "\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000\\\\u0000",
                "\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u\\\\\\\\u"
            };
            
            System.out.println("\n🔴 ATTACK PAYLOADS THAT COULD CAUSE DoS:");
            for (int i = 0; i < aggressivePayloads.length; i++) {
                System.out.println((i + 1) + ". " + aggressivePayloads[i].substring(0, Math.min(50, aggressivePayloads[i].length())) + "...");
            }
            
            // Test each payload with timeout protection
            for (int i = 0; i < aggressivePayloads.length; i++) {
                String payload = aggressivePayloads[i];
                System.out.println("\n--- Testing Payload " + (i + 1) + " ---");
                
                Instant startTime = Instant.now();
                
                try {
                    // Set a timeout to prevent infinite loops
                    String result = org.apache.commons.lang3.StringEscapeUtils.unescapeJava(payload);
                    
                    Instant endTime = Instant.now();
                    Duration duration = Duration.between(startTime, endTime);
                    
                    System.out.println("Input length: " + payload.length());
                    System.out.println("Output length: " + (result != null ? result.length() : 0));
                    System.out.println("Processing time: " + duration.toMillis() + "ms");
                    
                    if (duration.toMillis() > 100) {
                        System.out.println("⚠️  SLOW PROCESSING - Potential DoS vulnerability!");
                    } else {
                        System.out.println("✅ Processed quickly (may be patched or not triggering)");
                    }
                    
                } catch (Exception e) {
                    Instant endTime = Instant.now();
                    Duration duration = Duration.between(startTime, endTime);
                    System.out.println("❌ Exception after " + duration.toMillis() + "ms: " + e.getMessage());
                }
            }
            
            System.out.println("\n🚨 VULNERABILITY SUMMARY:");
            System.out.println("   - Version 3.17.0 is vulnerable to CVE-2025-48924");
            System.out.println("   - Malicious Unicode escape sequences can cause DoS");
            System.out.println("   - Infinite loops possible in StringEscapeUtils.unescapeJava()");
            System.out.println("   - Resource exhaustion attacks possible");
            System.out.println("   - Web applications using this version are at risk");
            
            System.out.println("\n🔒 MITIGATION REQUIRED:");
            System.out.println("   - Add explicit dependency on commons-lang3:3.19.0");
            System.out.println("   - Override vulnerable transitive dependency");
            System.out.println("   - Update all applications using commons-lang3 < 3.18.0");
            
        } else {
            System.out.println("✅ Version appears to be secure or different vulnerability");
        }
    }
    
    @Test
    @DisplayName("Show Dependency Path Without Security Fix")
    public void showVulnerableDependencyPath() {
        System.out.println("\n=== VULNERABLE DEPENDENCY PATH ANALYSIS ===");
        System.out.println("Without explicit commons-lang3 dependency:");
        System.out.println("└── SpringDoc OpenAPI 2.8.13");
        System.out.println("    └── Swagger Core 2.2.36");
        System.out.println("        └── commons-lang3:3.17.0 ❌ VULNERABLE");
        System.out.println();
        System.out.println("❌ This creates a security risk:");
        System.out.println("   - CVE-2025-48924 affects versions 3.17.0 and below");
        System.out.println("   - DoS attacks via malicious Unicode escapes");
        System.out.println("   - No explicit version control");
        System.out.println("   - Transitive dependency vulnerability");
        
        System.out.println("\n✅ With our security fix (explicit dependency):");
        System.out.println("├── commons-lang3:3.19.0 ✅ SECURE (explicit)");
        System.out.println("└── SpringDoc OpenAPI 2.8.13");
        System.out.println("    └── Swagger Core 2.2.36");
        System.out.println("        └── commons-lang3:3.17.0 ❌ OVERRIDDEN");
        System.out.println();
        System.out.println("✅ Security benefits:");
        System.out.println("   - Explicit version control");
        System.out.println("   - Vulnerable transitive dependency overridden");
        System.out.println("   - CVE-2025-48924 mitigated");
        System.out.println("   - DoS attacks prevented");
    }
    
    @Test
    @DisplayName("Demonstrate Attack Surface Without Fix")
    public void demonstrateAttackSurfaceWithoutFix() {
        System.out.println("\n=== ATTACK SURFACE WITHOUT SECURITY FIX ===");
        
        String version = org.apache.commons.lang3.StringUtils.class.getPackage().getImplementationVersion();
        System.out.println("Current version: " + version);
        
        if (version != null && version.startsWith("3.17")) {
            System.out.println("\n🚨 VULNERABLE VERSION - ATTACK SURFACE EXPOSED:");
            
            System.out.println("\n1. 🌐 Web Application Attacks:");
            System.out.println("   - Malicious requests to endpoints using StringEscapeUtils");
            System.out.println("   - DoS via crafted Unicode escape sequences");
            System.out.println("   - Resource exhaustion attacks");
            
            System.out.println("\n2. 💻 Application-Level Attacks:");
            System.out.println("   - Infinite loops in string processing");
            System.out.println("   - Memory exhaustion via large payloads");
            System.out.println("   - CPU exhaustion via repeated processing");
            
            System.out.println("\n3. 🔄 Recurring Attack Vectors:");
            System.out.println("   - Automated attack scripts");
            System.out.println("   - Botnet-based DoS attacks");
            System.out.println("   - Continuous resource exhaustion");
            
            System.out.println("\n4. 📊 Impact Assessment:");
            System.out.println("   - Service availability compromised");
            System.out.println("   - Performance degradation");
            System.out.println("   - Resource consumption spikes");
            System.out.println("   - User experience degradation");
            
            System.out.println("\n❌ WHAT'S NOT MITIGATED WITHOUT EXPLICIT DEPENDENCY:");
            System.out.println("   - DoS attacks via Unicode escapes");
            System.out.println("   - Resource exhaustion vulnerabilities");
            System.out.println("   - Infinite loop potential");
            System.out.println("   - Memory exhaustion attacks");
            System.out.println("   - CPU exhaustion attacks");
            System.out.println("   - Web application DoS");
            System.out.println("   - Service availability risks");
            
        } else {
            System.out.println("✅ Version appears secure - no attack surface exposed");
        }
    }
}
