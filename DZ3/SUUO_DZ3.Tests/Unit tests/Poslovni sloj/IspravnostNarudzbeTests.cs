using FluentValidation.TestHelper;
using SUUO_DZ3.Models;
using SUUO_DZ3.Models.Enums;
using SUUO_DZ3.Models.Validation;

namespace SUUO_DZ3.Tests.Unit_tests;

public class IspravnostNarudzbeTests
{
    private readonly NarudzbaValidator _validator;

    public IspravnostNarudzbeTests()
    {
        _validator = new NarudzbaValidator();
    }
    
    [Fact]
    public void Should_Pass_When_VrijemeNarudzbe_Is_Within_Working_Hours()
    {
        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2025, 5, 23, 15, 30, 0),
            Stol = "Stol01",
            Status = StatusNarudzbe.Zaprimljeno,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe { Status = StatusStavke.NaCekanju }
            }
        };

        var result = _validator.TestValidate(narudzba);
        result.ShouldNotHaveValidationErrorFor(x => x.VrijemeNarudzbe);
    }

    [Fact]
    public void Should_Fail_When_VrijemeNarudzbe_Is_Outside_Working_Hours()
    {
        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2025, 5, 23, 8, 30, 0),
            Stol = "Stol01",
            Status = StatusNarudzbe.UPripremi,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe { Status = StatusStavke.NaCekanju }
            }
        };

        var result = _validator.TestValidate(narudzba);
        result.ShouldHaveValidationErrorFor(x => x.VrijemeNarudzbe);
    }
    
    [Fact]
    public void Should_Fail_When_Stol_Is_In_Wrong_Format()
    {
        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2025, 5, 23, 12, 0, 0),
            Stol = "Stol86",
            Status = StatusNarudzbe.UPripremi,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe { Status = StatusStavke.NaCekanju, AkcijskaPonuda = true}
            }
        };

        var result = _validator.TestValidate(narudzba);
        result.ShouldHaveValidationErrorFor(x => x.Stol);
    }

    [Fact]
    public void Should_Pass_When_AllStavkeSuPripremljene_And_StatusJePosluzena()
    {
        var narudzba = new Narudzba()
        {
            VrijemeNarudzbe = new DateTime(2025, 5, 23, 12, 0, 0),
            Stol = "Stol01",
            Status = StatusNarudzbe.Posluzeno,
            MetodaPlacanja = MetodaPlacanja.Gotovina,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe { Status = StatusStavke.Pripremljeno },
                new StavkaNarudzbe { Status = StatusStavke.Pripremljeno }
            }
        };

        var result = _validator.TestValidate(narudzba);
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Should_Fail_When_AnyStavkaNijePripremljena_And_StatusJePosluzena()
    {
        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2025, 5, 23, 12, 0, 0),
            Stol = "Stol01",
            Status = StatusNarudzbe.Posluzeno,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe { Status = StatusStavke.NaCekanju },
                new StavkaNarudzbe { Status = StatusStavke.Pripremljeno }
            }
        };

        var result = _validator.TestValidate(narudzba);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Should_Fail_When_StavkeSuNaCekanju_And_StatusJeNaplacena()
    {
        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2025, 5, 23, 14, 0, 0),
            Stol = "Stol02",
            Status = StatusNarudzbe.Naplaceno,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe { Status = StatusStavke.NaCekanju },
            }
        };

        var result = _validator.TestValidate(narudzba);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Should_Pass_When_UkupnoAkcijskihPonuda_Is_Less_Than_3()
    {
        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2025, 5, 23, 12, 0, 0),
            Stol = "Stol01",
            Status = StatusNarudzbe.UPripremi,
            MetodaPlacanja = MetodaPlacanja.Kartica,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe { Status = StatusStavke.Pripremljeno, Cijena = 500},
                new StavkaNarudzbe { Status = StatusStavke.Pripremljeno, Cijena = 500}
            }
        };

        var result = _validator.TestValidate(narudzba);
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Fact]
    public void Should_Fail_If_CijenaJeBelow50_And_MetodaPlacanjaNijeGotovina()
    {
        var narudzba = new Narudzba
        {
            MetodaPlacanja = MetodaPlacanja.Kartica,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe { Status = StatusStavke.Pripremljeno, Cijena = 10},
                new StavkaNarudzbe { Status = StatusStavke.Pripremljeno, Cijena = 5}
            }
        };

        var result = _validator.TestValidate(narudzba);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Should_Pass_If_CijenajeIspod50_And_MetodaPlacanjaJeGotovina()
    {
        var narudzba = new Narudzba
        {
            MetodaPlacanja = MetodaPlacanja.Gotovina,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe { Status = StatusStavke.Pripremljeno, Cijena = 10},
                new StavkaNarudzbe { Status = StatusStavke.Pripremljeno, Cijena = 5}
            }
        };

        var result = _validator.TestValidate(narudzba);
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Should_Pass_If_CijenaJe50IVise()
    {
        var narudzba = new Narudzba
        {
            MetodaPlacanja = MetodaPlacanja.Kartica,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe { Status = StatusStavke.Pripremljeno, Cijena = 100},
                new StavkaNarudzbe { Status = StatusStavke.Pripremljeno, Cijena = 50}
            }
        };

        var result = _validator.TestValidate(narudzba);
        result.ShouldNotHaveValidationErrorFor(x => x.UkupnaCijena);
    }
}