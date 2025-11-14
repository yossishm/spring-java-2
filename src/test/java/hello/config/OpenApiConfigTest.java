package hello.config;

import io.swagger.v3.oas.models.OpenAPI;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.context.ApplicationContext;

import static org.junit.jupiter.api.Assertions.*;

@SpringBootTest
class OpenApiConfigTest {

    @Autowired
    private ApplicationContext applicationContext;

    @Test
    void testOpenApiBeanExists() {
        OpenAPI openAPI = applicationContext.getBean(OpenAPI.class);
        assertNotNull(openAPI, "OpenAPI bean should be created");
    }

    @Test
    void testOpenApiInfo() {
        OpenAPI openAPI = applicationContext.getBean(OpenAPI.class);
        assertNotNull(openAPI.getInfo(), "OpenAPI info should not be null");
        assertEquals("Spring Boot JWT Authorization API", openAPI.getInfo().getTitle());
        assertEquals("1.0.0", openAPI.getInfo().getVersion());
        assertNotNull(openAPI.getInfo().getDescription());
        assertTrue(openAPI.getInfo().getDescription().contains("JWT Authorization API"));
    }

    @Test
    void testOpenApiSecurityScheme() {
        OpenAPI openAPI = applicationContext.getBean(OpenAPI.class);
        assertNotNull(openAPI.getComponents(), "Components should not be null");
        assertNotNull(openAPI.getComponents().getSecuritySchemes(), "Security schemes should not be null");
        assertNotNull(openAPI.getComponents().getSecuritySchemes().get("bearer-jwt"), 
            "bearer-jwt security scheme should exist");
    }

    @Test
    void testOpenApiServers() {
        OpenAPI openAPI = applicationContext.getBean(OpenAPI.class);
        assertNotNull(openAPI.getServers(), "Servers should not be null");
        assertFalse(openAPI.getServers().isEmpty(), "At least one server should be configured");
        assertEquals(2, openAPI.getServers().size(), "Should have 2 servers configured");
    }

    @Test
    void testOpenApiSecurityRequirement() {
        OpenAPI openAPI = applicationContext.getBean(OpenAPI.class);
        assertNotNull(openAPI.getSecurity(), "Security requirements should not be null");
        assertFalse(openAPI.getSecurity().isEmpty(), "At least one security requirement should exist");
    }
}

