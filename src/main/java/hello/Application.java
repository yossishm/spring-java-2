package hello;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestHeader;
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

import java.util.Map;
import org.apache.commons.codec.binary.Base64;
import java.security.Key;
import java.security.KeyFactory;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.security.NoSuchAlgorithmException;
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

/**
 * Exception thrown when there's an error processing a private key.
 */
class PrivateKeyProcessingException extends RuntimeException {
    public PrivateKeyProcessingException(String message, Throwable cause) {
        super(message, cause);
    }
}

/**
 * Exception thrown when there's an error processing a public key.
 */
class PublicKeyProcessingException extends RuntimeException {
    public PublicKeyProcessingException(String message, Throwable cause) {
        super(message, cause);
    }
}

// OpenTelemetry imports removed - using Spring Boot native OTLP

@SpringBootApplication
@RestController
@Tag(name = "Cache Services", description = "Cache management operations with JWT authorization")
public class Application {

    private static final Logger logger = LoggerFactory.getLogger(Application.class);


   // Server Side - cache - getObject
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
    logger.info("Get operation called for object ID: [REDACTED]");
    return "get";
   }

  // Server Side - cache - putObject
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
    logger.info("Put operation called for object ID: [REDACTED]");
    return "put";
  }

  // Server Side - cache - deleteObject
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
    logger.info("Delete operation called for object ID: [REDACTED]");
    return "delete";
  }

  // Server Side
  @RequestMapping("/")
  public String home(@RequestHeader final Map<String, String> headers) {
    logger.info("JWT header processing completed");
    return "Hello Docker Yossi World";
  }


  public static void main(final String[] args) {
    SpringApplication.run(Application.class, args);
  }

  /**
   * Loads a private key from a DER file.
   * @param filename The path to the DER file.
   * @return The loaded PrivateKey.
   * @throws IOException if an I/O error occurs.
   * @throws NoSuchAlgorithmException if the RSA algorithm is not available.
   * @throws InvalidKeySpecException if the key specification is invalid.
   */
  public static PrivateKey loadPrivateKey(final String filename) throws IOException, NoSuchAlgorithmException, InvalidKeySpecException {

    final byte[] keyBytes = Files.readAllBytes(Paths.get(filename));

    final PKCS8EncodedKeySpec spec =
      new PKCS8EncodedKeySpec(keyBytes);
    final KeyFactory kf = KeyFactory.getInstance("RSA");
    return kf.generatePrivate(spec);
  }

  /**
   * Loads a public key from a DER file.
   * @param filename The path to the DER file.
   * @return The loaded PublicKey.
   * @throws IOException if an I/O error occurs.
   * @throws NoSuchAlgorithmException if the RSA algorithm is not available.
   * @throws InvalidKeySpecException if the key specification is invalid.
   */
  public static PublicKey loadPublicKey(final String filename) throws IOException, NoSuchAlgorithmException, InvalidKeySpecException {
    final byte[] keyBytes = Files.readAllBytes(Paths.get(filename));

    final X509EncodedKeySpec spec =
      new X509EncodedKeySpec(keyBytes);
    final KeyFactory kf = KeyFactory.getInstance("RSA");
    return kf.generatePublic(spec);
  }

  /**
   * Reads a key from an InputStream using a provided key parser function.
   * @param in The InputStream to read from.
   * @param keyParser A function to parse the key bytes.
   * @return The parsed Key.
   * @throws IOException if an I/O error occurs.
   */
  private static Key readKey(final InputStream in, final Function<byte[], Key> keyParser) throws IOException {
      final StringBuilder content = new StringBuilder();
      try (BufferedReader reader = new BufferedReader(new InputStreamReader(in))) {
          String line;
          while ((line = reader.readLine()) != null) {
              content.append(line);
          }
      }
      final String encoded = content.toString()
              .replaceAll("-----BEGIN (.*) KEY-----", "")
              .replaceAll("-----END (.*) KEY-----", "")
              .replaceAll("\\s", "");
      final byte[] keyBytes = Base64.decodeBase64(encoded);
      return keyParser.apply(keyBytes);
  }

  /**
   * Reads a private key from an InputStream.
   * @param in The InputStream to read from.
   * @return The loaded PrivateKey.
   * @throws IOException if an I/O error occurs.
   * @throws NoSuchAlgorithmException if the RSA algorithm is not available.
   * @throws InvalidKeySpecException if the key specification is invalid.
   */
  public static PrivateKey readPrivateKey(final InputStream in) throws IOException, NoSuchAlgorithmException {
      final KeyFactory keyFactory = KeyFactory.getInstance("RSA");
      return (PrivateKey) readKey(in, keyBytes -> {
          try {
              final PKCS8EncodedKeySpec keySpec = new PKCS8EncodedKeySpec(keyBytes);
              return keyFactory.generatePrivate(keySpec);
          } catch (InvalidKeySpecException e) {
              throw new PrivateKeyProcessingException("Invalid private key spec", e);
          }
      });
  }

  /**
   * Reads a public key from an InputStream.
   * @param in The InputStream to read from.
   * @return The loaded PublicKey.
   * @throws IOException if an I/O error occurs.
   * @throws NoSuchAlgorithmException if the RSA algorithm is not available.
   * @throws InvalidKeySpecException if the key specification is invalid.
   */
  public static PublicKey readPublicKey(final InputStream in) throws IOException, NoSuchAlgorithmException {
      final KeyFactory keyFactory = KeyFactory.getInstance("RSA");
      return (PublicKey) readKey(in, keyBytes -> {
          try {
              final X509EncodedKeySpec spec = new X509EncodedKeySpec(keyBytes);
              return keyFactory.generatePublic(spec);
          } catch (InvalidKeySpecException e) {
              throw new PublicKeyProcessingException("Invalid public key spec", e);
          }
      });
  }
}