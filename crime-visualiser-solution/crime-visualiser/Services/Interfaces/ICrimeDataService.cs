using crime_visualiser.Models;

namespace crime_visualiser.Services.Interfaces;

public interface ICrimeDataService
{
    public Task<IEnumerable<CrimeCategoryCount>?> GetCrimeCategoryCountsAsync(CrimeQuery crimeQuery);
}
