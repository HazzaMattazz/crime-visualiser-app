using System.Text.Json.Serialization;

namespace crime_visualiser.Models;

public record CrimeDto
{
    [JsonPropertyName("category")]
    public string? Category { get; init; }

    [JsonPropertyName("location_type")]
    public string? LocationType { get; init; }

    [JsonPropertyName("location")]
    public LocationDto? Location { get; init; }

    [JsonPropertyName("context")]
    public string? Context { get; init; }

    [JsonPropertyName("outcome_status")]
    public OutcomeStatusDto? OutcomeStatus { get; init; }

    [JsonPropertyName("persistent_id")]
    public string? PersistentId { get; init; }

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("location_subtype")]
    public string? LocationSubtype { get; init; }

    [JsonPropertyName("month")]
    public string? Month { get; init; }
}

public record LocationDto
{
    [JsonPropertyName("latitude")]
    public string? Latitude { get; init; }

    [JsonPropertyName("street")]
    public StreetDto? Street { get; init; }

    [JsonPropertyName("longitude")]
    public string? Longitude { get; init; }
}

public record StreetDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

public record OutcomeStatusDto
{
    [JsonPropertyName("category")]
    public string? Category { get; init; }

    [JsonPropertyName("date")]
    public string? Date { get; init; }
}
