namespace LibraryBlazor.Features.Auth.Models;

public sealed record RegisterRequestDto(string Username, string Password, string[]? Roles);
