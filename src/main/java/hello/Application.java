package hello;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import hello.security.RequirePermission;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.security.SecurityRequirement;
import io.swagger.v3.oas.annotations.tags.Tag;

import java.util.HashMap;
import java.util.Map;
import java.nio.charset.StandardCharsets;
import org.apache.commons.codec.binary.Base64;
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
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.nio.file.Files;
import java.nio.file.Paths;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

// OpenTelemetry imports removed - using Spring Boot native OTLP

@SpringBootApplication
@RestController
@Tag(name = "Cache Services", description = "Cache management operations with JWT authorization")
public class Application {

    private static final Logger logger = LoggerFactory.getLogger(Application.class);

  // OpenTelemetry bean removed - using Spring Boot native OTLP

   /// Server Side - cache - getObject
   @RequestMapping("/api/v1/cacheServices/getObject")
   @PreAuthorize("hasAuthority('CACHE_READ') or hasAuthority('CACHE_ADMIN')")
   @RequirePermission(value = {"CACHE_READ", "CACHE_ADMIN"})
   @Operation(
       summary = "Get cache object",
       description = "Retrieves an object from cache. Requires CACHE_READ or CACHE_ADMIN permission.",
       security = @SecurityRequirement(name = "bearer-jwt")
   )
   @ApiResponses(value = {
       @ApiResponse(responseCode = "200", description = "Object retrieved successfully"),
       @ApiResponse(responseCode = "401", description = "Unauthorized - Invalid or missing JWT token"),
       @ApiResponse(responseCode = "403", description = "Forbidden - Insufficient permissions (requires CACHE_READ or CACHE_ADMIN)")
   })
   public String getObject(
       @RequestHeader final Map<String, String> headers,
       @Parameter(description = "Cache object ID", required = true, example = "123")
       @RequestParam(name = "id") final String objectId) {  
    logger.info("Get: {} Called", objectId);
    return "get";
   }

  /// Server Side - cache - putObject
  //@RequestMapping("/api/v1/cacheServices/putObject")
  @PutMapping("/api/v1/cacheServices/putObject")
  @PreAuthorize("hasAuthority('CACHE_WRITE') or hasAuthority('CACHE_ADMIN')")
  @RequirePermission(value = {"CACHE_WRITE", "CACHE_ADMIN"})
  @Operation(
      summary = "Put cache object",
      description = "Stores an object in cache. Requires CACHE_WRITE or CACHE_ADMIN permission.",
      security = @SecurityRequirement(name = "bearer-jwt")
  )
  @ApiResponses(value = {
      @ApiResponse(responseCode = "200", description = "Object stored successfully"),
      @ApiResponse(responseCode = "401", description = "Unauthorized - Invalid or missing JWT token"),
      @ApiResponse(responseCode = "403", description = "Forbidden - Insufficient permissions (requires CACHE_WRITE or CACHE_ADMIN)")
  })
  public String putObject(
      @RequestHeader final Map<String, String> headers,
      @Parameter(description = "Cache object ID", required = true, example = "123")
      @RequestParam(name = "id") final String objectId) {
    logger.info("Put: {} Called", objectId);
    return "put";
  }

  /// Server Side - cache - deleteObject
  @DeleteMapping("/api/v1/cacheServices/deleteObject")
  @PreAuthorize("hasAuthority('CACHE_DELETE') or hasAuthority('CACHE_ADMIN')")
  @RequirePermission(value = {"CACHE_DELETE", "CACHE_ADMIN"})
  @Operation(
      summary = "Delete cache object",
      description = "Removes an object from cache. Requires CACHE_DELETE or CACHE_ADMIN permission.",
      security = @SecurityRequirement(name = "bearer-jwt")
  )
  @ApiResponses(value = {
      @ApiResponse(responseCode = "200", description = "Object deleted successfully"),
      @ApiResponse(responseCode = "401", description = "Unauthorized - Invalid or missing JWT token"),
      @ApiResponse(responseCode = "403", description = "Forbidden - Insufficient permissions (requires CACHE_DELETE or CACHE_ADMIN)")
  })
  public String deleteObject(
      @RequestHeader final Map<String, String> headers,
      @Parameter(description = "Cache object ID", required = true, example = "123")
      @RequestParam(name = "id") final String objectId) {
    logger.info("Delete: {} Called", objectId);
    return "delete";
  }

  /// Server Side
  @RequestMapping("/")
  public String home(@RequestHeader final Map<String, String> headers) {
    final String jwsHeader = java.util.Base64.getEncoder().encodeToString("Authorization: Bearer".getBytes(StandardCharsets.UTF_8));
    logger.info("jws header base64 is : {}", jwsHeader);

    // try {
    //   Key publicKey = loadPublicKey("/Users/yshmulev/dev/gs-spring-boot-docker/spring-java/public-istio-demo-pkcs.der");//new FileInputStream(
    //   headers.forEach((key, value) -> {
    //   System.out.println(String.format("Header '%s' = %s", key, value));
    //   Jws<Claims> jws;
      
      
    //   if (key.equalsIgnoreCase (jwsHeader)){
    //       String jwsString = value;
    //       try {
    //         jws = Jwts.parserBuilder()  // (1)
    //         .setSigningKey(publicKey)         // (2)
    //         .build()                    // (3)
    //         .parseClaimsJws(jwsString); // (4)
    //         System.out.println ("jws parsing is: " + jws);
            
    //         // we can safely trust the JWT

            
    //       }catch (JwtException ex) {       // (5)
            
    //         // we *cannot* use the JWT as intended by its creator
    //       }
    //     }
    //   });
      
    
 
    // }catch (Exception e){
    //   System.err.println (e.getStackTrace ());
    // }
      
    return "Hello Docker Yossi World";
  }


  public static void main(final String[] args) throws Exception {
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


      
  //     KeyFactory keyFactory = KeyFactory.getInstance("RSA");

  //     Key privateKey = loadPrivateKey("/Users/yshmulev/dev/gs-spring-boot-docker/spring-java/private-istio-demo-pkcs.der");//new FileInputStream());
  //     Key publicKey = loadPublicKey("/Users/yshmulev/dev/gs-spring-boot-docker/spring-java/public-istio-demo-pkcs.der");//new FileInputStream(

  //     System.out.println(publicKey);
  //     System.out.println(privateKey);

  //     String jwt = Jwts.builder()
  //             .setSubject("paul.simon")
  //             .signWith(SignatureAlgorithm.RS256, privateKey)
  //             .compact();

  //     System.out.println(jwt);

  //     String subject = Jwts.parser().setSigningKey(publicKey).parseClaimsJws(jwt).getBody().getSubject();
  //     System.out.println(subject);

  //     String jwsString = Jwts.builder().setSubject("Joe").signWith(privateKey).compact();
  //     System.out.println ("Token is: " + jwsString);

  //     assert Jwts.parserBuilder().setSigningKey(privateKey).build().parseClaimsJws(jwsString).getBody().getSubject().equals("Jo");





  // jwsString = Jwts.builder() // (1)

  //   .setSubject("Bob")      // (2) 

  //   .signWith(privateKey)          // (3)

  //   .setIssuer("me")
  //   .setSubject("Bob")
  //   .setAudience("you")
  //   //.setExpiration(expiration) //a java.util.Date
  //   //.setNotBefore(notBefore) //a java.util.Date s
  //   .setIssuedAt(new Date()) // for example, now
  //   .setId(UUID.randomUUID().toString ()) //just an example id


  //   .claim("yossi", "accoountdata")
  //   .claim("foo" , "bar")
     
  //   .compact();             // (4)

  //   System.out.println ("jws is: " + jwsString);




  //   // get response to myself
     LocalRestClient restClient = new LocalRestClient ("");//(jwsString);

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
    try (BufferedReader reader = new BufferedReader(new InputStreamReader(in, StandardCharsets.UTF_8))) {
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
       // PKCS8EncodedKeySpec keySpec = new PKCS8EncodedKeySpec(bytes);
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



