using FluentValidation;

namespace SUUO_DZ3.Models.Validation;

public class StavkaNarudbeValidator: AbstractValidator<StavkaNarudzbe>
{
    public StavkaNarudbeValidator()
    {
        RuleFor(x => x.Cijena)
            .GreaterThan(0)
            .WithMessage("Cijena mora biti veća od 0.");
        
        When(x => x.AkcijskaPonuda, () =>
        {
            RuleFor(x => x.Cijena)
                .LessThan(10)
                .WithMessage("Cijena akcijske ponude mora biti manja od 10.00 EUR.");

            RuleFor(x => x.Kolicina)
                .LessThanOrEqualTo(5)
                .WithMessage("Maksimalna količina za akcijsku ponudu je 5.");
        });
    }
}