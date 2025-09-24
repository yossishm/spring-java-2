package hello;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.springframework.http.HttpMethod;
import org.springframework.http.MediaType;
import org.springframework.test.util.ReflectionTestUtils;
import org.springframework.test.web.client.MockRestServiceServer;
import org.springframework.web.client.RestTemplate;

import java.nio.charset.StandardCharsets;
import java.util.Base64;

import static org.assertj.core.api.Assertions.assertThat;
import static org.springframework.test.web.client.match.MockRestRequestMatchers.header;
import static org.springframework.test.web.client.match.MockRestRequestMatchers.method;
import static org.springframework.test.web.client.match.MockRestRequestMatchers.requestTo;
import static org.springframework.test.web.client.response.MockRestResponseCreators.withSuccess;

class LocalRestClientTest {

    @Test
    @DisplayName("LocalRestClient GET and POST use headers and return body")
    void getAndPost_work() {
        LocalRestClient client = new LocalRestClient("Bearer abc");

        RestTemplate restTemplate = (RestTemplate) ReflectionTestUtils.getField(client, "rest");
        MockRestServiceServer server = MockRestServiceServer.createServer(restTemplate);

        String encodedHeaderName = Base64.getEncoder().encodeToString("Authorization: Bearer".getBytes(StandardCharsets.UTF_8));

        // Register both expectations before performing any requests
        server.expect(requestTo("http://localhost:8080/api/health"))
            .andExpect(method(HttpMethod.GET))
            .andExpect(header(encodedHeaderName, "Bearer abc"))
            .andRespond(withSuccess("ok", MediaType.TEXT_PLAIN));

        server.expect(requestTo("http://localhost:8080/api/test"))
            .andExpect(method(HttpMethod.POST))
            .andExpect(header(encodedHeaderName, "Bearer abc"))
            .andRespond(withSuccess("posted", MediaType.TEXT_PLAIN));

        String getBody = client.get("/api/health");
        assertThat(getBody).isEqualTo("ok");
        assertThat(client.getStatus()).isNotNull();

        String postBody = client.post("/api/test", "{}");
        assertThat(postBody).isEqualTo("posted");
        assertThat(client.getStatus()).isNotNull();

        server.verify();
    }
}


