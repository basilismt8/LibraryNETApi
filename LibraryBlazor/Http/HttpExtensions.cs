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

        services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        });

        services.AddScoped<ApiClient>();

        return services;
    }
}
