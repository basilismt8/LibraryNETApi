namespace LibraryBlazor.Auth;

public sealed record LoginRequest(string Username, string Password);

public sealed record LoginResponse(string AccessToken);
