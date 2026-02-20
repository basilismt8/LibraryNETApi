namespace LibraryBlazor.Features.Auth.Models;

public sealed class RegisterVm
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
