package hello;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.web.bind.annotation.*;
import java.util.*;
import java.time.*;
import java.nio.charset.StandardCharsets;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.Jws;
import io.jsonwebtoken.Claims;
import io.jsonwebtoken.JwtException;
import io.jsonwebtoken.SignatureAlgorithm;
import org.apache.commons.codec.binary.Base64;
import io.jsonwebtoken.security.Keys;
import java.security.Key;
import java.security.KeyFactory;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.security.NoSuchAlgorithmException;
import java.security.interfaces.RSAPrivateKey;
import java.security.spec.InvalidKeySpecException;
import java.security.spec.PKCS8EncodedKeySpec;
import java.security.spec.X509EncodedKeySpec;
import java.util.function.Function;


import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.FileNotFoundException;
import java.nio.file.*;

import org.springframework.web.reactive.function.client.ClientResponse;
import org.springframework.web.reactive.function.client.WebClient;
import org.springframework.web.reactive.function.client.WebClient.*;
import org.springframework.web.reactive.function.*;
import org.springframework.web.bind.annotation.*;
import reactor.core.publisher.Mono;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;


@SpringBootApplication
@RestController
public class Application {

   /// Server Side - cache - getObject
   @RequestMapping("/api/v1/cacheServices/getObject")
   public String getObject( @RequestHeader Map<String, String> headers , @RequestParam(name = "id") String id) {  
    System.out.println ("Get: " + id + " Called");
    return "get";
   }

  /// Server Side - cache - putObject
  //@RequestMapping("/api/v1/cacheServices/putObject")
  @PutMapping("/api/v1/cacheServices/putObject")
  public String putObject( @RequestHeader Map<String, String> headers , @RequestParam(name = "id") String id) {
    System.out.println ("Put: " + id + " Called");
    return "put";
  }

  /// Server Side - cache - putObject
  @DeleteMapping("/api/v1/cacheServices/deleteObject")
  public String deleteObject( @RequestHeader Map<String, String> headers , @RequestParam(name = "id") String id) {
    System.out.println ("Delete: " + id + " Called");
    return "delete";
  }

  /// Server Side
  @RequestMapping("/")
  public String home( @RequestHeader Map<String, String> headers) {
    String jwsHeader = java.util.Base64.getEncoder().encodeToString("Authorization: Bearer".getBytes(StandardCharsets.UTF_8));
    System.out.println ("jws header base64 is : " + jwsHeader);

    try {
      Key publicKey = loadPublicKey("/Users/yshmulev/dev/gs-spring-boot-docker/spring-java/public-istio-demo-pkcs.der");//new FileInputStream(
      headers.forEach((key, value) -> {
      System.out.println(String.format("Header '%s' = %s", key, value));
      Jws<Claims> jws;
      
      
      if (key.equalsIgnoreCase (jwsHeader)){
          String jwsString = value;
          try {
            jws = Jwts.parserBuilder()  // (1)
            .setSigningKey(publicKey)         // (2)
            .build()                    // (3)
            .parseClaimsJws(jwsString); // (4)
            System.out.println ("jws parsing is: " + jws);
            
            // we can safely trust the JWT

            
          }catch (JwtException ex) {       // (5)
            
            // we *cannot* use the JWT as intended by its creator
          }
        }
      });
      
    
 
    }catch (Exception e){
      System.err.println (e.getStackTrace ());
    }
      
    return "Hello Docker Yossi World";
  }


  public static void main(String[] args) throws Exception {
    SpringApplication.run(Application.class, args);


/// Client Side

  //openssl genrsa -out istio-demo.pem 512

  //You must make your PCKS8 file from your private key!

// private.pem => name of private key file
// openssl genrsa -out private-istio-demo.pem 2048
// public_key.pem => name of public key file
// openssl rsa -in private-istio-demo.pem -pubout -outform PEM -out public-istio-demo.pem
// ‫‪private_key.pem‬‬ => name of private key with PCKS8 format! you can just read this format in java
// openssl pkcs8 -topk8 -inform PEM -outform DER -in private-istio-demo.pem -out private-istio-demo-pkcs.der -nocrypt
// public key in der format
// openssl rsa -in private-istio-demo.pem -pubout -outform DER -out public-istio-demo-pkcs.der



      // We need a signing key, so we'll create one just for this example. Usually
      // the key would be read from your application configuration instead.
      //Key key = "/Users/yshmulev/dev/gs-spring-boot-docker/spring-java/istio-demo.pem"; //Keys.secretKeyFor(SignatureAlgorithm.HS256);

      KeyFactory keyFactory = KeyFactory.getInstance("RSA");

      Key privateKey = loadPrivateKey("/Users/yshmulev/dev/gs-spring-boot-docker/spring-java/private-istio-demo-pkcs.der");//new FileInputStream());
      Key publicKey = loadPublicKey("/Users/yshmulev/dev/gs-spring-boot-docker/spring-java/public-istio-demo-pkcs.der");//new FileInputStream(

      System.out.println(publicKey);
      System.out.println(privateKey);

      String jwt = Jwts.builder()
              .setSubject("paul.simon")
              .signWith(SignatureAlgorithm.RS256, privateKey)
              .compact();

      System.out.println(jwt);

      String subject = Jwts.parser().setSigningKey(publicKey).parseClaimsJws(jwt).getBody().getSubject();
      System.out.println(subject);

      String jwsString = Jwts.builder().setSubject("Joe").signWith(privateKey).compact();
      System.out.println ("Token is: " + jwsString);

      assert Jwts.parserBuilder().setSigningKey(privateKey).build().parseClaimsJws(jwsString).getBody().getSubject().equals("Jo");



//      String header = '{"alg":"HS256"}';
//      String claims = '{"sub":"Joe"}';

 //     String encodedHeader = base64URLEncode( header.getBytes("UTF-8") );
 //     String encodedClaims = base64URLEncode( claims.getBytes("UTF-8") );


 //     String concatenated = encodedHeader + '.' + encodedClaims;
      
     
 //     byte[] signature = hmacSha256( concatenated, privateKey );

 //     jws = concatenated + '.' + base64URLEncode( signature );

  //    System.out.println ("jws is: " + jws);


  jwsString = Jwts.builder() // (1)

    .setSubject("Bob")      // (2) 

    .signWith(privateKey)          // (3)

    .setIssuer("me")
    .setSubject("Bob")
    .setAudience("you")
    //.setExpiration(expiration) //a java.util.Date
    //.setNotBefore(notBefore) //a java.util.Date s
    .setIssuedAt(new Date()) // for example, now
    .setId(UUID.randomUUID().toString ()) //just an example id


    .claim("yossi", "accoountdata")
    .claim("foo" , "bar")
     
    .compact();             // (4)

    System.out.println ("jws is: " + jwsString);




    // get response to myself
    LocalRestClient restClient = new LocalRestClient (jwsString);

    String response = restClient.get("");
    System.out.println ("local host called . status is:" + restClient.getStatus() + "response is: " + response);
  }

  public static PrivateKey loadPrivateKey(String filename) throws Exception {

    byte[] keyBytes = Files.readAllBytes(Paths.get(filename));

    PKCS8EncodedKeySpec spec =
      new PKCS8EncodedKeySpec(keyBytes);
    KeyFactory kf = KeyFactory.getInstance("RSA");
    return kf.generatePrivate(spec);
  }

  public static PublicKey loadPublicKey (String filename)throws Exception {
    
    byte[] keyBytes = Files.readAllBytes(Paths.get(filename));

    X509EncodedKeySpec spec =
      new X509EncodedKeySpec(keyBytes);
    KeyFactory kf = KeyFactory.getInstance("RSA");
    return kf.generatePublic(spec);
  }

  private static Key loadKey(InputStream in, Function<byte[], Key> keyParser) throws IOException {
    try (BufferedReader reader = new BufferedReader(new InputStreamReader(in))) {
        String line;
        StringBuilder content = new StringBuilder();
        while ((line = reader.readLine()) != null) {
            if (!(line.contains("BEGIN") || line.contains("END"))) {
                content.append(line).append('\n');
            }
        }
        byte[] encoded = Base64.decodeBase64(content.toString());
        return keyParser.apply(encoded);
    }
}

public static Key loadPrivateKey(InputStream in) throws IOException, NoSuchAlgorithmException {
    KeyFactory keyFactory = KeyFactory.getInstance("RSA");

    return loadKey(in, bytes -> {
        PKCS8EncodedKeySpec keySpec = new PKCS8EncodedKeySpec(bytes);
        try {
            RSAPrivateKey key = (RSAPrivateKey) keyFactory.generatePrivate(keySpec);
            return key;
        } catch (InvalidKeySpecException e) {
            throw new RuntimeException(e);
        }
    });
}

public static Key loadPublicKey(InputStream in) throws IOException, NoSuchAlgorithmException {
    KeyFactory keyFactory = KeyFactory.getInstance("RSA");

    return loadKey(in, bytes -> {
        PKCS8EncodedKeySpec keySpec = new PKCS8EncodedKeySpec(bytes);
        try {
            X509EncodedKeySpec spec =
                    new X509EncodedKeySpec(bytes);
            return keyFactory.generatePublic(spec);
        } catch (InvalidKeySpecException e) {
            throw new RuntimeException(e);
        }
    });
}
  
}



