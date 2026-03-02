# crime-visualiser-app

Basic ASP.NET Blazor application to visualise crime data in the UK. The application uses the Police API to retrieve crime data based on geographic coordinates and display it interactively.

## Project structure

- `crime-visualiser/` - main Blazor application
  - `Components/Pages/CrimePage.razor` - main UI for searching and displaying category counts
  - `Components/Pages/CrimePage.razor.cs` - backing logic: validation, calling the service, rendering state
  - `Services/CrimeDataService.cs` - service that groups and counts crimes by category
  - `HttpClients/UkCrimeDataClient.cs` - `HttpClient`-based client that calls the Police API and deserializes `CrimeDto`
  - `Models/` - domain types and DTOs (`CrimeDto`, `CrimeCategoryCount`, `CrimeQuery`, etc.)
  - `Models/Validators/CrimeQueryValidator.cs` - request validation using FluentValidation

- `crime-visualiser.tests/` - unit and component tests
  - `Services/CrimeDataServiceTests.cs` - unit tests for service logic (uses `Moq`, `xUnit`)
  - `Components/CrimePageTests.cs` - bUnit tests for the `CrimePage` component
  - `HttpClients/UkCrimeDataClientTests.cs` - tests for the HTTP client

## Key libraries and tools

- `.NET 10` and C# 14 - target framework and language
- `bUnit` - Blazor component testing
- `xUnit` - unit test runner
- `Moq` - mocking dependencies in tests
- `FluentValidation` - request/model validation used by the page

## Version

- Current: 1.0

## Releases / Changelog

- `1.0` - Initial release: basic Blazor UI, crime data client, grouping/counting service, validation and tests.


