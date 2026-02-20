using LibraryBlazor.Features.Auth.Models;
using LibraryBlazor.Http;

namespace LibraryBlazor.Features.Auth.Services;

public sealed class AuthApi
{
    private readonly ApiClient _api;

    public AuthApi(ApiClient api)
    {
        _api = api;
    }

    public Task<ApiResult<LoginResponseDto>> LoginAsync(LoginRequestDto body, CancellationToken cancellationToken = default)
        => _api.PostResultAsync<LoginResponseDto, object>("api/auth/login", new
        {
            username = body.Username,
            password = body.Password
        }, cancellationToken);

    public Task<ApiResult<string>> RegisterAsync(RegisterRequestDto body, CancellationToken cancellationToken = default)
        => _api.PostResultAsync<string, object>("api/auth/register", new
        {
            username = body.Username,
            password = body.Password,
            roles = body.Roles
        }, cancellationToken);
}
