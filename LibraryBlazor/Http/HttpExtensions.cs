using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryBlazor.Http;

public static class HttpExtensions
{
    public static IServiceCollection AddApiHttp(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration["Api:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("Missing configuration key 'Api:BaseUrl'.");
        }

        services.AddScoped<TokenHandler>();

        services.AddHttpClient<ApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
        }).AddHttpMessageHandler<TokenHandler>();

        return services;
    }
}
