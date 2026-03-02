using crime_visualiser.HttpClients.Interfaces;
using crime_visualiser.Models;

namespace crime_visualiser.HttpClients;

public class UkCrimeDataClient(HttpClient httpClient, ILogger<UkCrimeDataClient> logger) : IUkCrimeDataClient
{
    public async Task<IEnumerable<CrimeDto>?> GetCrimesAsync(double latitude, double longitude, DateOnly date)
    {
        try
        {
            ValidateParameters(latitude, longitude, date);

            var dateString = date.ToString("yyyy-MM");
            var endpoint = $"crimes-street/all-crime?lat={latitude}&lng={longitude}&date={dateString}";

            var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<IEnumerable<CrimeDto>>();
            return content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching crime data for lat: {Latitude}, lng: {Longitude}, date: {Date}", latitude, longitude, date);
            return null;
        }
    }

    private static void ValidateParameters(double latitude, double longitude, DateOnly date)
    {
        if (latitude < -90 || latitude > 90) throw new ArgumentOutOfRangeException(nameof(latitude));
        if (longitude < -180 || longitude > 180) throw new ArgumentOutOfRangeException(nameof(longitude));
        if (date == default) throw new ArgumentException("Date must be provided.", nameof(date));
    }
}
