using crime_visualiser.Models;

namespace crime_visualiser.HttpClients.Interfaces;

public interface IUkCrimeDataClient
{
    Task<IEnumerable<CrimeDto>?> GetCrimesAsync(double latitude, double longitude, DateOnly date);
}