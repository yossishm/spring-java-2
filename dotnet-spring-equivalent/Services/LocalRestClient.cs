// <copyright file="LocalRestClient.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Services;

using System.Text;
using System.Text.Json;

/// <summary>
/// Equivalent to Spring's LocalRestClient for making HTTP requests
/// </summary>
public class LocalRestClient : IDisposable
{
    private readonly HttpClient httpClient;
    private readonly string server = "http://localhost:8080";

    public LocalRestClient(string authorization = "")
    {
        this.httpClient = new HttpClient();
        this.httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
        this.httpClient.DefaultRequestHeaders.Add("Accept", "*/*");

        // Equivalent to Spring's base64 encoded authorization header
        var jwsHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes("Authorization: Bearer"));
        this.httpClient.DefaultRequestHeaders.Add(jwsHeader, authorization);
    }

    /// <summary>
    /// Make a GET request - equivalent to Spring's get method
    /// </summary>
    public async Task<string> GetAsync(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        
        try
        {
            var response = await this.httpClient.GetAsync(new Uri(this.server + uri.ToString())).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
    public async Task<string> PostAsync(Uri uri, string json)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(json);
        
        try
        {
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await this.httpClient.PostAsync(new Uri(this.server + uri.ToString()), content).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
    public async Task<string> PutAsync(Uri uri, string json)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(json);
        
        try
        {
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await this.httpClient.PutAsync(new Uri(this.server + uri.ToString()), content).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
    public async Task<string> DeleteAsync(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        
        try
        {
            var response = await this.httpClient.DeleteAsync(new Uri(this.server + uri.ToString())).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return content;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"DELETE request failed: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.httpClient?.Dispose();
        }
    }
}