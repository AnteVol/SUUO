using System.Text.RegularExpressions;
using FluentValidation;

namespace SUUO_DZ3.Models.Validation;

public class NarudzbaValidator : AbstractValidator<Narudzba>
{
    public NarudzbaValidator()
    {
        RuleFor(x => x.VrijemeNarudzbe)
            .Must(UnutarRadnogVremena)
            .WithMessage("Narudžbe se zaprijamju samo između 10:00 i 23:00.");

        RuleFor(x => x.UkupnoAkcijskihPonuda)
            .LessThan(3)
            .WithMessage("Narudžbe može sadržavati najviše 3 proizvoda sa akcijskom ponudom.");
        
        RuleFor(x => x.Stol)
            .Must(IspravnostStola)
            .WithMessage("Naziv stola mora biti u formatu 'StolXX', gdje je XX broj od 00 do 25.");
    }

    private bool UnutarRadnogVremena(DateTime vrijeme)
    {
        var pocetak = new TimeSpan(10, 0, 0);
        var kraj = new TimeSpan(23, 0, 0);

        var vrijemeNarudzbe = vrijeme.TimeOfDay;
        return vrijemeNarudzbe >= pocetak && vrijemeNarudzbe <= kraj;
    }

    private bool IspravnostStola(string stol)
    {
        if (string.IsNullOrEmpty(stol))
            return false;

        return Regex.IsMatch(stol, @"^Stol(0[0-9]|1[0-9]|2[0-5])$");
    }
}