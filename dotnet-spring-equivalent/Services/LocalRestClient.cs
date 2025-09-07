using System.Text;
using System.Text.Json;

namespace SpringJavaEquivalent.Services;

/// <summary>
/// Equivalent to Spring's LocalRestClient for making HTTP requests
/// </summary>
public class LocalRestClient
{
    private readonly HttpClient _httpClient;
    private readonly string _server = "http://localhost:8080";

    public LocalRestClient(string authorization = "")
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");

        // Equivalent to Spring's base64 encoded authorization header
        var jwsHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes("Authorization: Bearer"));
        _httpClient.DefaultRequestHeaders.Add(jwsHeader, authorization);
    }

    /// <summary>
    /// Make a GET request - equivalent to Spring's get method
    /// </summary>
    public async Task<string> GetAsync(string uri)
    {
        try
        {
            var response = await _httpClient.GetAsync(_server + uri);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"GET request failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Make a POST request - equivalent to Spring's post method
    /// </summary>
    public async Task<string> PostAsync(string uri, string json)
    {
        try
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_server + uri, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"POST request failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Make a PUT request
    /// </summary>
    public async Task<string> PutAsync(string uri, string json)
    {
        try
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(_server + uri, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"PUT request failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Make a DELETE request
    /// </summary>
    public async Task<string> DeleteAsync(string uri)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(_server + uri);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"DELETE request failed: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}