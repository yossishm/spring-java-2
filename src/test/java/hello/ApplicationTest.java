package hello;

import org.junit.jupiter.api.Test;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.context.SpringBootTest.WebEnvironment;
import org.springframework.boot.test.web.client.TestRestTemplate;
import org.springframework.boot.test.web.server.LocalServerPort;
import org.springframework.context.annotation.Import;

import static org.assertj.core.api.Assertions.assertThat;

@SpringBootTest(webEnvironment = WebEnvironment.RANDOM_PORT, properties = "spring.profiles.active=test")
@Import(TestSecurityConfig.class)
public class ApplicationTest {

	@LocalServerPort
	private int port;

	@Autowired
	private TestRestTemplate restTemplate;

	@Test
	public void greetingShouldReturnDefaultMessage() throws Exception {
		assertThat(this.restTemplate.getForObject("http://localhost:" + port + "/",
				String.class)).contains("Hello Docker Yossi World");
	}

	@Test
	public void greetingCacheAdd() throws Exception {
		assertThat(this.restTemplate.getForObject("http://localhost:" + port + "/api/v1/cacheServices/putObject?id=1",
				String.class)).contains("put");
	}

	@Test
	public void greetingCacheGet() throws Exception {
		assertThat(this.restTemplate.getForObject("http://localhost:" + port + "/api/v1/cacheServices/getObject?id=1",
				String.class)).contains("get");
	}

	@Test
	public void greetingCacheDelete() throws Exception {
		assertThat(this.restTemplate.getForObject("http://localhost:" + port + "/api/v1/cacheServices/deleteObject?id=1",
				String.class)).contains("delete");
	}
}

