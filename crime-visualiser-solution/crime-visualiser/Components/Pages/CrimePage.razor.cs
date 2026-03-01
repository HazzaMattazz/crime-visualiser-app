using crime_visualiser.HttpClients.Interfaces;
using crime_visualiser.Models;
using crime_visualiser.Models.Validators;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Collections;

namespace crime_visualiser.Components.Pages;

public partial class CrimePage(IUkCrimeDataClient _ukCrimeDataClient)
{
    private CrimeQuery crimeQuery;

    private bool hasSearched = false;
    private IEnumerable<CrimeDto>? crimeData;
    private IEnumerable<CrimeCategoryCount>? categoryCounts;

    protected override void OnInitialized()
    {
        // Initialize with sensible defaults so the UI has values to bind to
        crimeQuery = new CrimeQuery
        {
            Latitude = 51.44237,
            Longitude = -2.49810,
            Date = DateOnly.FromDateTime(DateTime.Today)
        };
    }

    protected async Task OnSearchClicked()
    {
        if (!ValidateCrimeQuery(crimeQuery))
        {
            // Handle validation failure (e.g., show error message)
            return;
        }

        crimeData = await GetCrimeDataAsync(crimeQuery);

        // Group the returned crime data by category and compute counts
        if (crimeData is not null)
        {
            categoryCounts = crimeData
                .GroupBy(c => string.IsNullOrWhiteSpace(c?.Category) ? "Unknown" : c.Category!)
                .Select(g => new CrimeCategoryCount(g.Key, g.Count()))
                .OrderByDescending(x => x.Count)
                .ToList();
        }

        hasSearched = true;
    }

    private bool ValidateCrimeQuery(CrimeQuery crimeQuery)
    {
        var validator = new CrimeQueryValidator();
        var validationResult = validator.Validate(crimeQuery);
        return validationResult.IsValid;
    }

    private async Task<IEnumerable<CrimeDto>?> GetCrimeDataAsync(CrimeQuery crimeQuery)
    {
        return await _ukCrimeDataClient.GetCrimesAsync(crimeQuery.Latitude, crimeQuery.Longitude, crimeQuery.Date);
    }

    private record CrimeCategoryCount(string Category, int Count);
}
