using LibraryBlazor;
using LibraryBlazor.Auth;
using LibraryBlazor.Features.Auth.Services;
using LibraryBlazor.Features.Books.Services;
using LibraryBlazor.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddApiHttp(builder.Configuration);

builder.Services.AddScoped<TokenService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());

builder.Services.AddScoped<AuthApi>();
builder.Services.AddScoped<BookApi>();

await builder.Build().RunAsync();
