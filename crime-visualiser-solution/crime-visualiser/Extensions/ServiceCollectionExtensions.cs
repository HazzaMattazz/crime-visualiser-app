using crime_visualiser.Constants;
using crime_visualiser.HttpClients;
using crime_visualiser.HttpClients.Interfaces;
using System.Security.Authentication;

namespace crime_visualiser.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration.GetValue<string>($"ApiBaseUrls:{HttpClientNames.CrimeDataApi}");
        if (baseUrl == null ) {
            throw new InvalidOperationException($"Base URL for {HttpClientNames.CrimeDataApi} is not configured.");
        }

        services.AddHttpClient<IUkCrimeDataClient, UkCrimeDataClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            SslOptions = { EnabledSslProtocols = SslProtocols.Tls13 },
        });

        return services;
    }
}
