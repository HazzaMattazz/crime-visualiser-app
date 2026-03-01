using crime_visualiser.Services.Interfaces;

namespace crime_visualiser.Services;

public class CrimeDataService(IHttpClientFactory httpClientFactory) : ICrimeDataService
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Constants.HttpClientNames.CrimeDataApi);

    public async Task GetCrime()
    {

    }
}
