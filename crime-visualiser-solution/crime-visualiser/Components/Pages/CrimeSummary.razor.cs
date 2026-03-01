using crime_visualiser.HttpClients.Interfaces;

namespace crime_visualiser.Components.Pages;

public partial class CrimeSummary(IUkCrimeDataClient _ukCrimeDataClient)
{
    protected override async void OnInitialized()
    {
        var response = await _ukCrimeDataClient.GetCrimesAsync(51.44237, -2.49810, new DateOnly(2025, 1, 1));
    }
}
