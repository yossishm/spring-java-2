package hello;

import org.springframework.boot.test.context.TestConfiguration;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Profile;

@TestConfiguration
@Profile("test")
public class TestSecurityConfig {

    // Simple test configuration without Spring Security
    @Bean
    public String testBean() {
        return "test-configuration";
    }
}
