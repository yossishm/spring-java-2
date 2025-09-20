package hello.config;

import io.swagger.v3.oas.models.Components;
import io.swagger.v3.oas.models.OpenAPI;
import io.swagger.v3.oas.models.info.Contact;
import io.swagger.v3.oas.models.info.Info;
import io.swagger.v3.oas.models.info.License;
import io.swagger.v3.oas.models.security.SecurityRequirement;
import io.swagger.v3.oas.models.security.SecurityScheme;
import io.swagger.v3.oas.models.servers.Server;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import java.util.List;

/**
 * OpenAPI/Swagger configuration with JWT authorization support.
 * Provides comprehensive API documentation with authorization levels.
 */
@Configuration
public class OpenApiConfig {

    @Bean
    public OpenAPI customOpenAPI() {
        return new OpenAPI()
                .info(new Info()
                        .title("Spring Boot JWT Authorization API")
                        .description("""
                                ## JWT Authorization API Documentation
                                
                                This API demonstrates comprehensive JWT-based authorization with multiple security levels:
                                
                                ### Authorization Levels:
                                1. **Public Access** - No authentication required
                                2. **Authentication Required** - Valid JWT token needed
                                3. **Role-Based Access** - Specific roles required (ADMIN, USER)
                                4. **Permission-Based Access** - Specific permissions required
                                5. **Hierarchical Permissions** - Multiple permission combinations
                                
                                ### Available Permissions:
                                - `CACHE_READ` - Read cache operations
                                - `CACHE_WRITE` - Write cache operations  
                                - `CACHE_DELETE` - Delete cache operations
                                - `CACHE_ADMIN` - Full cache administration
                                
                                ### Available Roles:
                                - `ADMIN` - Full system access
                                - `USER` - Basic user access
                                
                                ### Authentication Assurance Levels (AAL):
                                - `AAL1` - Single-factor authentication (password only)
                                - `AAL2` - Multi-factor authentication (password + MFA)
                                - `AAL3` - Hardware-based authentication (FIDO2, smart cards)
                                
                                ### Identity Providers (IDP):
                                - `local` - Local authentication
                                - `azure-ad` - Microsoft Azure Active Directory
                                - `okta` - Okta Identity Provider
                                - `enterprise-ldap` - Enterprise LDAP
                                
                                ### Getting Started:
                                1. Generate a JWT token using `/api/v1/auth/token/{type}`
                                2. Use the token in the Authorization header: `Bearer <token>`
                                3. Access protected endpoints based on your permissions
                                """)
                        .version("1.0.0")
                        .contact(new Contact()
                                .name("API Support")
                                .email("support@example.com")
                                .url("https://example.com"))
                        .license(new License()
                                .name("Apache 2.0")
                                .url("https://www.apache.org/licenses/LICENSE-2.0.html")))
                .servers(List.of(
                        new Server()
                                .url("http://localhost:8080")
                                .description("Development server"),
                        new Server()
                                .url("https://api.example.com")
                                .description("Production server")))
                .components(new Components()
                        .addSecuritySchemes("bearer-jwt", new SecurityScheme()
                                .type(SecurityScheme.Type.HTTP)
                                .scheme("bearer")
                                .bearerFormat("JWT")
                                .description("""
                                        JWT Authorization header using the Bearer scheme.
                                        
                                        **Example:** `Authorization: Bearer <token>`
                                        
                                        **Token Generation:**
                                        - Use `/api/v1/auth/token/admin` for admin access
                                        - Use `/api/v1/auth/token/user` for basic user access
                                        - Use `/api/v1/auth/token/cache-reader` for read-only access
                                        - Use `/api/v1/auth/token/cache-writer` for read/write access
                                        - Use `/api/v1/auth/token/cache-admin` for full cache access
                                        """)))
                .addSecurityItem(new SecurityRequirement().addList("bearer-jwt"));
    }
}
