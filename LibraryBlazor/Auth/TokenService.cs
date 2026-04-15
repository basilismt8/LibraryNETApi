using Microsoft.JSInterop;
using System.Text.Json;

namespace LibraryBlazor.Auth;

public sealed class TokenService
{
    private const string TokenKey = "auth.token";
    private const string EmailKey = "auth.email";

    private readonly IJSRuntime _js;

    public TokenService(IJSRuntime js)
    {
        _js = js;
    }

    public ValueTask SetTokenAsync(string token, bool persist)
    {
        var storage = persist ? "localStorage" : "sessionStorage";
        return _js.InvokeVoidAsync($"{storage}.setItem", TokenKey, token);
    }

    public ValueTask SetRememberedEmailAsync(string email)
        => _js.InvokeVoidAsync("localStorage.setItem", EmailKey, email);

    public ValueTask<string?> GetRememberedEmailAsync()
        => _js.InvokeAsync<string?>("localStorage.getItem", EmailKey);

    public async ValueTask<string?> GetTokenAsync()
    {
        var token = await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(token)) return token;

        return await _js.InvokeAsync<string?>("sessionStorage.getItem", TokenKey).ConfigureAwait(false);
    }

    public async ValueTask<string?> GetValidTokenAsync()
    {
        var token = await GetTokenAsync().ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        if (IsTokenExpired(token))
        {
            await ClearTokenAsync().ConfigureAwait(false);
            return null;
        }

        return token;
    }

    public bool IsTokenExpired(string token)
    {
        var exp = TryGetExpiryUtc(token);
        if (exp is null)
        {
            return true;
        }

        return exp.Value <= DateTimeOffset.UtcNow;
    }

    private static DateTimeOffset? TryGetExpiryUtc(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length < 2)
            {
                return null;
            }

            var jsonBytes = Base64UrlDecode(parts[1]);
            using var doc = JsonDocument.Parse(jsonBytes);
            if (!doc.RootElement.TryGetProperty("exp", out var expProp))
            {
                return null;
            }

            if (expProp.ValueKind == JsonValueKind.Number && expProp.TryGetInt64(out var seconds))
            {
                return DateTimeOffset.FromUnixTimeSeconds(seconds);
            }

            if (expProp.ValueKind == JsonValueKind.String && long.TryParse(expProp.GetString(), out seconds))
            {
                return DateTimeOffset.FromUnixTimeSeconds(seconds);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static byte[] Base64UrlDecode(string input)
    {
        input = input.Replace('-', '+').Replace('_', '/');
        switch (input.Length % 4)
        {
            case 2: input += "=="; break;
            case 3: input += "="; break;
        }

        return Convert.FromBase64String(input);
    }

    public async ValueTask ClearTokenAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey).ConfigureAwait(false);
        await _js.InvokeVoidAsync("sessionStorage.removeItem", TokenKey).ConfigureAwait(false);
    }

    public ValueTask ClearRememberedEmailAsync()
        => _js.InvokeVoidAsync("localStorage.removeItem", EmailKey);
}
