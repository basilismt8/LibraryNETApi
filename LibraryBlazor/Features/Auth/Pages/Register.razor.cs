using LibraryBlazor.Auth;
using LibraryBlazor.Features.Auth.Models;
using LibraryBlazor.Features.Auth.Services;
using Microsoft.AspNetCore.Components;

namespace LibraryBlazor.Features.Auth.Pages;

public partial class Register
{
    [Inject] public AuthApi AuthApi { get; set; } = default!;
    [Inject] public JwtAuthStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] public NavigationManager Nav { get; set; } = default!;

    protected RegisterVm Model { get; } = new();
    protected bool _saving;
    protected string? _error;

    protected async Task RegisterAsync()
    {
        if (_saving)
            return;

        _error = null;
        _saving = true;

        try
        {
            if (string.IsNullOrWhiteSpace(Model.Email) || string.IsNullOrWhiteSpace(Model.Password) || string.IsNullOrWhiteSpace(Model.ConfirmPassword))
            {
                _error = "Email, password, and confirm password are required.";
                return;
            }

            if (!string.Equals(Model.Password, Model.ConfirmPassword, StringComparison.Ordinal))
            {
                _error = "Password and confirm password must match.";
                return;
            }

            var register = await AuthApi.RegisterAsync(
                new RegisterRequestDto(Model.Email.Trim(), Model.Password, Roles: ["Member"]))
                .ConfigureAwait(false);

            if (!register.Success)
            {
                _error = register.Message ?? "Registration failed.";
                return;
            }

            var login = await AuthApi.LoginAsync(new LoginRequestDto(Model.Email.Trim(), Model.Password))
                .ConfigureAwait(false);

            if (!login.Success)
            {
                Nav.NavigateTo("/login", replace: true);
                return;
            }

            var token = login.Data?.JwtToken;
            if (string.IsNullOrWhiteSpace(token))
            {
                Nav.NavigateTo("/login", replace: true);
                return;
            }

            await AuthStateProvider.SignInAsync(token).ConfigureAwait(false);
            Nav.NavigateTo("/books", replace: true);
        }
        finally
        {
            _saving = false;
        }
    }
}
