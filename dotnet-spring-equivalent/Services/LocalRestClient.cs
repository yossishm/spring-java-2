// <copyright file="LocalRestClient.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

using System.Text;
using System.Text.Json;

namespace SpringJavaEquivalent.Services;

/// <summary>
/// Equivalent to Spring's LocalRestClient for making HTTP requests
/// </summary>
public class LocalRestClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _server = "http://localhost:8080";

    public LocalRestClient(string authorization = "")
    {
        this._httpClient = new HttpClient();
        this._httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
        this._httpClient.DefaultRequestHeaders.Add("Accept", "*/*");

        // Equivalent to Spring's base64 encoded authorization header
        var jwsHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes("Authorization: Bearer"));
        this._httpClient.DefaultRequestHeaders.Add(jwsHeader, authorization);
    }

    /// <summary>
    /// Make a GET request - equivalent to Spring's get method
    /// </summary>
    public async Task<string> GetAsync(Uri uri)
    {
        try
        {
            var response = await this._httpClient.GetAsync(new Uri(this._server + uri.ToString())).ConfigureAwait(false);
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
        try
        {
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await this._httpClient.PostAsync(new Uri(this._server + uri.ToString()), content).ConfigureAwait(false);
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
        try
        {
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await this._httpClient.PutAsync(new Uri(this._server + uri.ToString()), content).ConfigureAwait(false);
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
        try
        {
            var response = await this._httpClient.DeleteAsync(new Uri(this._server + uri.ToString())).ConfigureAwait(false);
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
        this._httpClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}