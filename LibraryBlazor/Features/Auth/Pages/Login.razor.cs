using LibraryBlazor.Auth;
using LibraryBlazor.Features.Auth.Models;
using LibraryBlazor.Features.Auth.Services;
using Microsoft.AspNetCore.Components;

namespace LibraryBlazor.Features.Auth.Pages;

public partial class Login
{
    [Inject] public AuthApi AuthApi { get; set; } = default!;
    [Inject] public JwtAuthStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] public NavigationManager Nav { get; set; } = default!;
    [Inject] public TokenService TokenService { get; set; } = default!;

    protected LoginVm Model { get; } = new();
    protected bool _saving;
    protected string? _error;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthStateProvider.GetAuthenticationStateAsync();
        if (state.User.Identity?.IsAuthenticated == true)
        {
            Nav.NavigateTo("/books", replace: true);
            return;
        }

        var rememberedEmail = await TokenService.GetRememberedEmailAsync();
        if (!string.IsNullOrWhiteSpace(rememberedEmail))
        {
            Model.Email = rememberedEmail;
            Model.RememberMe = true;
        }
    }

    protected async Task SignInAsync()
    {
        _error = null;
        _saving = true;

        try
        {
            if (string.IsNullOrWhiteSpace(Model.Email) || string.IsNullOrWhiteSpace(Model.Password))
            {
                _error = "Email and password are required.";
                return;
            }

            var result = await AuthApi.LoginAsync(new LoginRequestDto(Model.Email.Trim(), Model.Password));
            if (!result.Success)
            {
                _error = result.Message ?? "Invalid login attempt";
                return;
            }

            var token = result.Data?.JwtToken;
            if (string.IsNullOrWhiteSpace(token))
            {
                _error = "Login succeeded but no token was returned.";
                return;
            }

            if (Model.RememberMe)
                await TokenService.SetRememberedEmailAsync(Model.Email.Trim());
            else
                await TokenService.ClearRememberedEmailAsync();

            await AuthStateProvider.SignInAsync(token, Model.RememberMe);
            Nav.NavigateTo("/books", forceLoad: false);
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _saving = false;
        }
    }
}
