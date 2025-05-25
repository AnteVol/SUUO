using System.Text.RegularExpressions;
using FluentValidation;
using SUUO_DZ3.Models.Enums;

namespace SUUO_DZ3.Models.Validation;

public class NarudzbaValidator : AbstractValidator<Narudzba>
{
    public NarudzbaValidator()
    {
        RuleFor(x => x.VrijemeNarudzbe)
            .Must(UnutarRadnogVremena)
            .WithMessage("Narudžbe se zaprijamju samo između 10:00 i 23:00.");

        RuleFor(x => x.Stol)
            .Must(IspravnostStola)
            .WithMessage("Naziv stola mora biti u formatu 'StolXX', gdje je XX broj od 00 do 25.");

        RuleFor(x => x)
            .Must(SveStavkePripremljeneAkoStatusPosluzenIliNaplacen)
            .WithMessage(
                "Sve stavke narudžbe moraju biti pripremljene prije nego što narudžba može biti poslužena ili naplaćena.");

        RuleFor(x => x)
            .Must(MetodaPlacanjaValidacija)
            .WithMessage(
                "Metoda plaćanja mora biti prazna dok sve stavke nisu poslužene. Kada su sve stavke poslužene, za iznose do 50 može se platiti samo gotovinom.");
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

    private bool SveStavkePripremljeneAkoStatusPosluzenIliNaplacen(Narudzba narudzba)
    {
        if (narudzba.Status != StatusNarudzbe.Posluzeno &&
            narudzba.Status != StatusNarudzbe.Naplaceno)
            return true;

        return narudzba.StavkeNarudzbi.All(stavka => stavka.Status == StatusStavke.Pripremljeno);
    }

    private bool MetodaPlacanjaValidacija(Narudzba narudzba)
    {
        bool sveStavkePripremljene = 
            narudzba.StavkeNarudzbi != null && 
            narudzba.StavkeNarudzbi.Any() && 
            narudzba.StavkeNarudzbi.All(stavka => stavka.Status == StatusStavke.Pripremljeno);
        
        if (narudzba.StavkeNarudzbi == null || narudzba.StavkeNarudzbi.Count == 0)
        {
            return narudzba.MetodaPlacanja == null;
        }

        if (!sveStavkePripremljene)
        {
            return narudzba.MetodaPlacanja == null;
        }

        if (narudzba.UkupnaCijena <= 50)
        {
            return narudzba.MetodaPlacanja == MetodaPlacanja.Gotovina;
        }

        return narudzba.MetodaPlacanja != null;
    }
}