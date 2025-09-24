package hello;

import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.security.Keys;
import io.jsonwebtoken.SignatureAlgorithm;
import java.security.Key;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class JWTtest {
    private static final Logger logger = LoggerFactory.getLogger(JWTtest.class);
    
    public static void main(final String[] args) {
        // We need a signing key, so we'll create one just for this example. Usually
        // the key would be read from your application configuration instead.
        final Key key = Keys.secretKeyFor(SignatureAlgorithm.HS256);

        final String jws = Jwts.builder()
            .subject("Joe")
            .signWith(key)
            .compact();
        logger.info("Generated JWT: {}", jws);
    }
}
