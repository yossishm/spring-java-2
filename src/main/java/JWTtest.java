

import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.SignatureAlgorithm;
import io.jsonwebtoken.security.Keys;
import java.security.Key;

public class JWTtest {

    
    public static void main(String[] args) {
        //HMAC
     //   Algorithm algorithmHS = Algorithm.HMAC256("secret");

        //RSA
       // RSAPublicKey publicKey = (RSAPublicKey) PemUtils.readPublicKeyFromFile("/path/to/rsa/key.pem", "RSA"); //Get the key instance
       // RSAPrivateKey privateKey = (RSAPrivateKey)  PemUtils.readPrivateKeyFromFile ("/path/to/rsa/key.pem", "RSA") ; //Get the key instance

        
        
        // Algorithm algorithmRS = Algorithm.RSA256(publicKey, privateKey);





        // We need a signing key, so we'll create one just for this example. Usually
        // the key would be read from your application configuration instead.
        Key key = Keys.secretKeyFor(SignatureAlgorithm.HS256);

        String jws = Jwts.builder().setSubject("Joe").signWith(key).compact();
        System.out.println("Generated JWT: " + jws);
    }
}
