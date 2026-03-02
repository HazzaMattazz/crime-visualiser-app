namespace crime_visualiser.Models;

public record CrimeQuery
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateOnly Date { get; set; }
}
