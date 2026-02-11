using LibraryBlazor.Auth;
using LibraryBlazor.Features.Books.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

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

    public async Task<ApiResult<T>> GetResultAsync<T>(string relativeUrl, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
        await AddAuthHeaderAsync(request).ConfigureAwait(false);

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var fail = await EnsureSuccessAsync(response).ConfigureAwait(false);
            return ApiResult<T>.Fail(fail.Message, fail.ValidationErrors);
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return ApiResult<T>.Ok(default);
        }

        var data = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false);
        return ApiResult<T>.Ok(data);
    }

    public async Task<ApiResult> PostAsync<TBody>(
    string relativeUrl,
    TBody body,
    CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl)
        {
            Content = JsonContent.Create(body)
        };

        await AddAuthHeaderAsync(request);

        using var response = await _http.SendAsync(request, cancellationToken);
        return await EnsureSuccessAsync(response);
    }

    public async Task<ApiResult> PutAsync<TBody>(
    string relativeUrl,
    TBody body,
    CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, relativeUrl)
        {
            Content = JsonContent.Create(body)
        };

        await AddAuthHeaderAsync(request);

        using var response = await _http.SendAsync(request, cancellationToken);
        return await EnsureSuccessAsync(response);
    }

    public async Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, relativeUrl);
        await AddAuthHeaderAsync(request).ConfigureAwait(false);

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
    }

    public async Task<ApiResult> DeleteResultAsync(string relativeUrl, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, relativeUrl);
        await AddAuthHeaderAsync(request).ConfigureAwait(false);

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await EnsureSuccessAsync(response).ConfigureAwait(false);
    }

    private async Task AddAuthHeaderAsync(HttpRequestMessage request)
    {
        var token = await _tokenService.GetTokenAsync().ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task<ApiResult> EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return ApiResult.Ok();

        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var validation = JsonSerializer.Deserialize<ValidationErrorResponse>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (validation?.Errors != null)
            {
                return ApiResult.Fail(
                    validation.Title,
                    validation.Errors);
            }
        }
        catch
        {
            // ignore parsing error
        }

        return ApiResult.Fail(content);
    }
}
