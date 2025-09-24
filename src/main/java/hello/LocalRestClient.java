package hello;

import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpMethod;
import org.springframework.http.HttpStatusCode;
import org.springframework.http.ResponseEntity;
import org.springframework.web.client.RestTemplate;

import java.util.Base64;
import java.nio.charset.StandardCharsets;

public class LocalRestClient {

  private String server = "http://localhost:8080";
  private RestTemplate rest;
  private HttpHeaders headers;
  private HttpStatusCode status;

  public LocalRestClient(String autorization) {
    this.rest = new RestTemplate();
    this.headers = new HttpHeaders();
    headers.add("Content-Type", "application/json");
    headers.add("Accept", "*/*");
    
    headers.add(Base64.getEncoder().encodeToString(
      "Authorization: Bearer".getBytes(StandardCharsets.UTF_8)), autorization);
  }

  public String get(String uri) {
    HttpEntity<String> requestEntity = new HttpEntity<>("", headers);
    ResponseEntity<String> responseEntity = rest.exchange(server + uri, HttpMethod.GET, requestEntity, String.class);
    this.setStatusCode(responseEntity.getStatusCode());
    return responseEntity.getBody();
  }

  public String post(String uri, String json) {   
    HttpEntity<String> requestEntity = new HttpEntity<>(json, headers);
    ResponseEntity<String> responseEntity = rest.exchange(server + uri, HttpMethod.POST, requestEntity, String.class);
    this.setStatusCode(responseEntity.getStatusCode());
    return responseEntity.getBody();
  }


  public HttpStatusCode getStatus() {
    return status;
  }

  public void setStatusCode(HttpStatusCode status) {
    this.status = status;
  } 
}