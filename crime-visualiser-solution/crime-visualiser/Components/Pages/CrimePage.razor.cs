using crime_visualiser.Models;
using crime_visualiser.Models.Validators;
using crime_visualiser.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;

namespace crime_visualiser.Components.Pages;

public partial class CrimePage
{
    [Inject] private ILogger<CrimePage> Logger { get; set; } = default!;
    private readonly ICrimeDataService _crimeDataService;

    // Default location is Bristol city centre
    private CrimeQuery _crimeQuery = new CrimeQuery
    {
        Latitude = 51.45052,
        Longitude = -2.59470,
        Date = DateOnly.FromDateTime(DateTime.Today)
    };

    private bool _hasSearched = false;
    private bool _isLoading = false;
    private IEnumerable<CrimeCategoryCount>? _categoryCounts;

    private EditContext _editContext;
    private ValidationMessageStore _messageStore;

    public CrimePage(ICrimeDataService crimeDataService)
    {
        _crimeDataService = crimeDataService;

        _editContext = new EditContext(_crimeQuery);
        _messageStore = new ValidationMessageStore(_editContext);

        // Clear messages for a field when it changes
        _editContext.OnFieldChanged += (sender, e) => _messageStore.Clear(e.FieldIdentifier);
    }

    protected async Task OnSearchClicked()
    {
        if (!ValidateCrimeQuery(_crimeQuery))
            return;

        _isLoading = true;

        await GetCrimeCategoryDataAsync(_crimeQuery);

        _hasSearched = true;
        _isLoading = false;
    }

    private bool ValidateCrimeQuery(CrimeQuery crimeQuery)
    {
        // Clear previous messages
        _messageStore.Clear();

        var validator = new CrimeQueryValidator();
        ValidationResult validationResult = validator.Validate(crimeQuery);

        if (!validationResult.IsValid)
        {
            foreach (var failure in validationResult.Errors)
            {
                // Map the property name to the field identifier on the model
                var fieldIdentifier = new FieldIdentifier(crimeQuery, failure.PropertyName);
                _messageStore.Add(fieldIdentifier, failure.ErrorMessage);
            }

            // Notify the UI that validation state changed
            _editContext.NotifyValidationStateChanged();
        }

        return validationResult.IsValid;
    }

    private async Task GetCrimeCategoryDataAsync(CrimeQuery crimeQuery)
    {
        try
        {
            _categoryCounts = await _crimeDataService.GetCrimeCategoryCountsAsync(crimeQuery);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error fetching crime category data for query: {crimeQuery}", crimeQuery);
        }        
    }
}
