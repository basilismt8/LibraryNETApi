using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LibraryBlazor.Auth;

namespace LibraryBlazor.Http;

public sealed class ApiClient
{
    private readonly HttpClient _http;
    private readonly TokenService _tokenService;

    public ApiClient(HttpClient http, TokenService tokenService)
    {
        _http = http;
        _tokenService = tokenService;
    }

    public async Task<T?> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
        await AddAuthHeaderAsync(request).ConfigureAwait(false);

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task PostAsync<TBody>(string relativeUrl, TBody body, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl)
        {
            Content = JsonContent.Create(body)
        };

        await AddAuthHeaderAsync(request).ConfigureAwait(false);

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
    }

    public async Task PutAsync<TBody>(string relativeUrl, TBody body, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, relativeUrl)
        {
            Content = JsonContent.Create(body)
        };

        await AddAuthHeaderAsync(request).ConfigureAwait(false);

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
    }

    public async Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, relativeUrl);
        await AddAuthHeaderAsync(request).ConfigureAwait(false);

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
    }

    private async Task AddAuthHeaderAsync(HttpRequestMessage request)
    {
        var token = await _tokenService.GetTokenAsync().ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = response.Content is null
            ? null
            : await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        throw new ApiException(response.StatusCode, body);
    }
}

public sealed class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ApiException(HttpStatusCode statusCode, string? responseBody)
        : base(BuildMessage(statusCode, responseBody))
    {
        StatusCode = statusCode;
    }

    private static string BuildMessage(HttpStatusCode statusCode, string? responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return $"API request failed ({(int)statusCode} {statusCode}).";
        }

        return $"API request failed ({(int)statusCode} {statusCode}): {responseBody}";
    }
}
