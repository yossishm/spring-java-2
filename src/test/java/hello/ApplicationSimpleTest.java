package hello;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.junit.jupiter.MockitoExtension;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.nio.charset.StandardCharsets;

import static org.assertj.core.api.Assertions.assertThat;
import static org.assertj.core.api.Assertions.assertThatThrownBy;

@ExtendWith(MockitoExtension.class)
class ApplicationSimpleTest {

    @Test
    void testApplicationClassExists() {
        // Test that Application class exists and has the required structure
        assertThat(Application.class).isNotNull();
    }

    @Test
    void testApplicationHasMainMethod() {
        // Test that main method exists
        try {
            var mainMethod = Application.class.getMethod("main", String[].class);
            assertThat(mainMethod).isNotNull();
        } catch (NoSuchMethodException e) {
            assertThat(false).isTrue(); // Fail the test if main method doesn't exist
        }
    }

    @Test
    void testApplicationHasCacheMethods() {
        // Test that Application class has the cache service methods
        try {
            var getObjectMethod = Application.class.getMethod("getObject", java.util.Map.class, String.class);
            var putObjectMethod = Application.class.getMethod("putObject", java.util.Map.class, String.class);
            var deleteObjectMethod = Application.class.getMethod("deleteObject", java.util.Map.class, String.class);
            var homeMethod = Application.class.getMethod("home", java.util.Map.class);
            
            assertThat(getObjectMethod).isNotNull();
            assertThat(putObjectMethod).isNotNull();
            assertThat(deleteObjectMethod).isNotNull();
            assertThat(homeMethod).isNotNull();
        } catch (NoSuchMethodException e) {
            assertThat(false).isTrue(); // Fail the test if methods don't exist
        }
    }

    @Test
    void testApplicationHasKeyMethods() {
        // Test that Application class has the key loading methods
        try {
            var loadPrivateKeyMethod = Application.class.getMethod("loadPrivateKey", String.class);
            var loadPublicKeyMethod = Application.class.getMethod("loadPublicKey", String.class);
            var readPrivateKeyMethod = Application.class.getMethod("readPrivateKey", java.io.InputStream.class);
            var readPublicKeyMethod = Application.class.getMethod("readPublicKey", java.io.InputStream.class);
            
            assertThat(loadPrivateKeyMethod).isNotNull();
            assertThat(loadPublicKeyMethod).isNotNull();
            assertThat(readPrivateKeyMethod).isNotNull();
            assertThat(readPublicKeyMethod).isNotNull();
        } catch (NoSuchMethodException e) {
            assertThat(false).isTrue(); // Fail the test if methods don't exist
        }
    }

    @Test
    void testLoadPrivateKeyWithInvalidFile() {
        // Given
        String invalidFilePath = "/nonexistent/path/to/key.der";

        // When & Then
        assertThatThrownBy(() -> Application.loadPrivateKey(invalidFilePath))
            .isInstanceOf(IOException.class);
    }

    @Test
    void testLoadPublicKeyWithInvalidFile() {
        // Given
        String invalidFilePath = "/nonexistent/path/to/key.der";

        // When & Then
        assertThatThrownBy(() -> Application.loadPublicKey(invalidFilePath))
            .isInstanceOf(IOException.class);
    }

    @Test
    void testReadPrivateKeyWithInvalidInputStream() {
        // Given
        ByteArrayInputStream invalidStream = new ByteArrayInputStream("invalid content".getBytes());

        // When & Then
        assertThatThrownBy(() -> Application.readPrivateKey(invalidStream))
            .isInstanceOf(Exception.class);
    }

    @Test
    void testReadPublicKeyWithInvalidInputStream() {
        // Given
        ByteArrayInputStream invalidStream = new ByteArrayInputStream("invalid content".getBytes());

        // When & Then
        assertThatThrownBy(() -> Application.readPublicKey(invalidStream))
            .isInstanceOf(Exception.class);
    }
}
