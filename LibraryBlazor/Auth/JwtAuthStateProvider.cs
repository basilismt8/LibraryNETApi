using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace LibraryBlazor.Auth;

public sealed class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly TokenService _tokenService;

    public JwtAuthStateProvider(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenService.GetValidTokenAsync().ConfigureAwait(false);
        var principal = BuildPrincipalFromToken(token);
        return new AuthenticationState(principal);
    }

    public async Task SignInAsync(string accessToken, bool rememberMe)
    {
        await _tokenService.SetTokenAsync(accessToken, rememberMe).ConfigureAwait(false);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task SignOutAsync()
    {
        await _tokenService.ClearTokenAsync().ConfigureAwait(false);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static ClaimsPrincipal BuildPrincipalFromToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        try
        {
            var claims = ReadClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, authenticationType: "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }
    }

    private static IEnumerable<Claim> ReadClaimsFromJwt(string token)
    {
        var parts = token.Split('.');
        if (parts.Length < 2)
        {
            return Array.Empty<Claim>();
        }

        var jsonBytes = Base64UrlDecode(parts[1]);
        using var doc = JsonDocument.Parse(jsonBytes);

        var claims = new List<Claim>();
        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            if (prop.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in prop.Value.EnumerateArray())
                {
                    claims.Add(new Claim(prop.Name, item.ToString()));
                }
            }
            else
            {
                claims.Add(new Claim(prop.Name, prop.Value.ToString()));
            }
        }

        return claims;
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
}
