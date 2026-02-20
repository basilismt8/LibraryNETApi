using System.Net.Http.Headers;
using LibraryBlazor.Auth;

namespace LibraryBlazor.Http;

public sealed class TokenHandler : DelegatingHandler
{
    private readonly TokenService _tokenService;

    public TokenHandler(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetValidTokenAsync().ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
