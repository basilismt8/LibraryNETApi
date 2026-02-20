using Microsoft.JSInterop;
using System.Text.Json;

namespace LibraryBlazor.Auth;

public sealed class TokenService
{
    private const string TokenKey = "auth.token";

    private readonly IJSRuntime _js;

    public TokenService(IJSRuntime js)
    {
        _js = js;
    }

    public ValueTask SetTokenAsync(string token)
        => _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);

    public ValueTask<string?> GetTokenAsync()
        => _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);

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

    public ValueTask ClearTokenAsync()
        => _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
}
