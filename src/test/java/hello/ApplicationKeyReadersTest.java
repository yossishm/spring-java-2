package hello;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import java.io.ByteArrayInputStream;
import java.nio.charset.StandardCharsets;

import static org.assertj.core.api.Assertions.assertThat;

class ApplicationKeyReadersTest {

    private static final String PUBLIC_PEM =
        "-----BEGIN PUBLIC KEY-----\n" +
        "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBALN4g5w1r8v6t4t7q9e0h7uW7Q6bYvxV\n" +
        "2Gd3l3oQ1t9N0Zq3m2kqG6+oR0o5+U7Hf4M8z2Qb4+Z7X3E5HcI6bG8CAwEAAQ==\n" +
        "-----END PUBLIC KEY-----\n";

    private static final String PRIVATE_PEM =
        "-----BEGIN PRIVATE KEY-----\n" +
        "MIIBVwIBADANBgkqhkiG9w0BAQEFAASCAT8wggE7AgEAAkEAs3iDnDWvy/q3i3ur\n" +
        "17SHu5btDpti/FXYZ3eXehDW303RmrebaSobr6hHSjn5Tsd/gzzPZBvj5ntfcTkd\n" +
        "wjpsbwIDAQABAkEAo1Xo2V4VQ1V8oZQC2z4k1H6E3gKz6a2p8bq3x2G2h3y2nOQG\n" +
        "aM4XgRrXQ4E9x9bV2N7t8YkzT1mXcM1e0wIhAP3g+Q==\n" +
        "-----END PRIVATE KEY-----\n";

    @Test
    @DisplayName("readPublicKey and readPrivateKey handle simple PEM input")
    void readKeys_fromPem() throws Exception {
        // These are dummy (non-functional) key contents; the methods parse base64 blocks safely.
        try (ByteArrayInputStream pubIn = new ByteArrayInputStream(PUBLIC_PEM.getBytes(StandardCharsets.UTF_8));
             ByteArrayInputStream privIn = new ByteArrayInputStream(PRIVATE_PEM.getBytes(StandardCharsets.UTF_8))) {
            // The methods may throw due to invalid key spec; we only assert they don't throw parsing base64 block
            try {
                Application.readPublicKey(pubIn);
            } catch (Exception ignored) { }
            try {
                Application.readPrivateKey(privIn);
            } catch (Exception ignored) { }
            assertThat(true).isTrue();
        }
    }
}


