using crime_visualiser.HttpClients.Interfaces;
using crime_visualiser.Models;
using crime_visualiser.Services.Interfaces;

namespace crime_visualiser.Services;

public class CrimeDataService(IUkCrimeDataClient ukCrimeDataClient) : ICrimeDataService
{
    public async Task<IEnumerable<CrimeCategoryCount>?> GetCrimeCategoryCountsAsync(CrimeQuery crimeQuery)
    {
        var crimeData = await ukCrimeDataClient.GetCrimesAsync(crimeQuery.Latitude, crimeQuery.Longitude, crimeQuery.Date);

        if (crimeData == null)
            return null;

        var categoryCounts = crimeData
            .GroupBy(c => string.IsNullOrWhiteSpace(c?.Category) ? "Unknown" : c.Category!)
            .Select(g => new CrimeCategoryCount(g.Key, g.Count()))
            .OrderByDescending(x => x.Count)
            .ToList();

        return categoryCounts;
    }
}
