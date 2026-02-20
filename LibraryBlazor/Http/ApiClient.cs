using LibraryBlazor.Features.Books.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace LibraryBlazor.Http;

public sealed class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<T?> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);

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

        using var response = await _http.SendAsync(request, cancellationToken);
        return await EnsureSuccessAsync(response);
    }

    public async Task<ApiResult<TResponse>> PostResultAsync<TResponse, TBody>(
    string relativeUrl,
    TBody body,
    CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl)
        {
            Content = JsonContent.Create(body)
        };

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var fail = await EnsureSuccessAsync(response).ConfigureAwait(false);
            return ApiResult<TResponse>.Fail(fail.Message, fail.ValidationErrors);
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return ApiResult<TResponse>.Ok(default);
        }

        var data = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
        return ApiResult<TResponse>.Ok(data);
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

        using var response = await _http.SendAsync(request, cancellationToken);
        return await EnsureSuccessAsync(response);
    }

    public async Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, relativeUrl);

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
    }

    public async Task<ApiResult> DeleteResultAsync(string relativeUrl, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, relativeUrl);

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await EnsureSuccessAsync(response).ConfigureAwait(false);
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
