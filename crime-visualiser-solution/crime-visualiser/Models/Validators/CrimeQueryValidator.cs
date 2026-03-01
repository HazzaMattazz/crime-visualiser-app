using FluentValidation;

namespace crime_visualiser.Models.Validators;

public class CrimeQueryValidator : AbstractValidator<CrimeQuery>
{
    public CrimeQueryValidator()
    {
        RuleFor(x => x.Latitude)
            .NotNull()
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .NotNull()
            .InclusiveBetween(-180, 180);

        RuleFor(x => x.Date)
            .NotNull()
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));
    }
}
