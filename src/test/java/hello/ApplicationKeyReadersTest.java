package hello;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import java.io.ByteArrayInputStream;
import java.nio.charset.StandardCharsets;



class ApplicationKeyReadersTest {

    private static final String PUBLIC_PEM = """
        -----BEGIN PUBLIC KEY-----
        MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBALN4g5w1r8v6t4t7q9e0h7uW7Q6bYvxV
        2Gd3l3oQ1t9N0Zq3m2kqG6+oR0o5+U7Hf4M8z2Qb4+Z7X3E5HcI6bG8CAwEAAQ==
        -----END PUBLIC KEY-----
        """;

    private static final String PRIVATE_PEM = """
        -----BEGIN PRIVATE KEY-----
        MIIBVwIBADANBgkqhkiG9w0BAQEFAASCAT8wggE7AgEAAkEAs3iDnDWvy/q3i3ur
        17SHu5btDpti/FXYZ3eXehDW303RmrebaSobr6hHSjn5Tsd/gzzPZBvj5ntfcTkd
        wjpsbwIDAQABAkEAo1Xo2V4VQ1V8oZQC2z4k1H6E3gKz6a2p8bq3x2G2h3y2nOQG
        aM4XgRrXQ4E9x9bV2N7t8YkzT1mXcM1e0wIhAP3g+Q==
        -----END PRIVATE KEY-----
        """;

    @Test
    @DisplayName("readPublicKey throws PublicKeyProcessingException for invalid PEM")
    void readPublicKey_invalidPem_throws() {
        try (ByteArrayInputStream pubIn = new ByteArrayInputStream(PUBLIC_PEM.getBytes(StandardCharsets.UTF_8))) {
            org.assertj.core.api.Assertions.assertThatThrownBy(() -> Application.readPublicKey(pubIn))
                .as("Invalid public PEM should produce PublicKeyProcessingException")
                .isInstanceOf(PublicKeyProcessingException.class);
        } catch (Exception e) {
            // This catch is only to satisfy try-with-resources close; the assertion above is the purpose.
            throw new RuntimeException(e);
        }
    }

    @Test
    @DisplayName("readPrivateKey throws PrivateKeyProcessingException for invalid PEM")
    void readPrivateKey_invalidPem_throws() {
        try (ByteArrayInputStream privIn = new ByteArrayInputStream(PRIVATE_PEM.getBytes(StandardCharsets.UTF_8))) {
            org.assertj.core.api.Assertions.assertThatThrownBy(() -> Application.readPrivateKey(privIn))
                .as("Invalid private PEM should produce PrivateKeyProcessingException")
                .isInstanceOf(PrivateKeyProcessingException.class);
        } catch (Exception e) {
            // This catch is only to satisfy try-with-resources close; the assertion above is the purpose.
            throw new RuntimeException(e);
        }
    }
}


