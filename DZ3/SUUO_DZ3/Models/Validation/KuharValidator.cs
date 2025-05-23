using FluentValidation;

namespace SUUO_DZ3.Models.Validation;

public class KuharValidator: AbstractValidator<Kuhar>
{
    public KuharValidator()
    {
        RuleFor(k => k.Telefon)
            .Matches(@"^\+385-\d{9}$")
            .WithMessage("Telefon mora biti u formatu +385-XX-XXXXXXX.");

        RuleFor(k => k.Email)
            .EmailAddress()
            .WithMessage("Email je u neispravnom formatu.");

        RuleFor(k => k.Specijaliteti)
            .NotEmpty();
    }
}