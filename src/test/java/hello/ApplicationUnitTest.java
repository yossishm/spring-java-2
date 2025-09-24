package hello;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import java.util.HashMap;
import java.util.Map;

import static org.assertj.core.api.Assertions.assertThat;

class ApplicationUnitTest {

    @Test
    @DisplayName("Application cache endpoints return strings")
    void cacheEndpoints_returnValues() {
        Application app = new Application();
        Map<String, String> headers = new HashMap<>();

        String g = app.getObject(headers, "1");
        String p = app.putObject(headers, "1");
        String d = app.deleteObject(headers, "1");
        String h = app.home(headers);

        assertThat(g).isEqualTo("get");
        assertThat(p).isEqualTo("put");
        assertThat(d).isEqualTo("delete");
        assertThat(h).contains("Hello Docker");
    }
}


