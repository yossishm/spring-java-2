package hello.security;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * Custom annotation for method-level permission checking.
 * Can be used alongside Spring Security annotations for fine-grained authorization.
 */
@Target({ElementType.METHOD, ElementType.TYPE})
@Retention(RetentionPolicy.RUNTIME)
public @interface RequirePermission {
    String[] value() default {};
    String[] roles() default {};
    boolean requireAll() default false; // If true, user must have ALL permissions/roles
}


