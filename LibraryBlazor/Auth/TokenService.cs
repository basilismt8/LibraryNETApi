using Microsoft.JSInterop;

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

    public ValueTask ClearTokenAsync()
        => _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
}
