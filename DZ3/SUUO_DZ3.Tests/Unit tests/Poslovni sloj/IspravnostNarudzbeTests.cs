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
    public void ValidnaNarudzba_ShouldPassValidation()
    {
        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2024, 12, 15, 14, 30, 0),
            Stol = "Stol05",
            Status = StatusNarudzbe.Naplaceno,
            MetodaPlacanja = MetodaPlacanja.Gotovina,
            KonobarId = Guid.NewGuid(),
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe
                {
                    StavkaNarudzbeId = Guid.NewGuid(),
                    Naziv = "Pizza Margherita",
                    Kolicina = 1,
                    Cijena = 25.00m,
                    AkcijskaPonuda = false,
                    Status = StatusStavke.Pripremljeno
                },
                new StavkaNarudzbe
                {
                    StavkaNarudzbeId = Guid.NewGuid(),
                    Naziv = "Coca Cola",
                    Kolicina = 2,
                    Cijena = 3.50m,
                    AkcijskaPonuda = false,
                    Status = StatusStavke.Pripremljeno
                }
            }
        };
        
        var result = _validator.TestValidate(narudzba);

        result.ShouldNotHaveAnyValidationErrors();
        
        Assert.Equal(32.00m, narudzba.UkupnaCijena);
        Assert.Equal(2, narudzba.BrojStavki);
        Assert.Equal(0, narudzba.UkupnoAkcijskihPonuda);
    }

    [Fact]
    public void Narudzba_IzvanRadnogVremena_ShouldFailValidation()
    {
        // Arrange
        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2024, 12, 15, 9, 30, 0), // 09:30 - izvan radnog vremena
            Stol = "Stol05",
            Status = StatusNarudzbe.UPripremi,
            StavkeNarudzbi = new List<StavkaNarudzbe>()
        };

        // Act & Assert
        var result = _validator.TestValidate(narudzba);
        result.ShouldHaveValidationErrorFor(x => x.VrijemeNarudzbe)
              .WithErrorMessage("Narudžbe se zaprijamju samo između 10:00 i 23:00.");
    }

    [Fact]
    public void Narudzba_MetodaPlacanjaSetovanaAStavkeNisuPripremljene_ShouldFailValidation()
    {
        // Arrange
        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2024, 12, 15, 14, 30, 0),
            Stol = "Stol10",
            Status = StatusNarudzbe.UPripremi,
            MetodaPlacanja = MetodaPlacanja.Gotovina,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe
                {
                    Naziv = "Burger",
                    Kolicina = 1,
                    Cijena = 15.00m,
                    Status = StatusStavke.NaCekanju 
                }
            }
        };
        
        var result = _validator.TestValidate(narudzba);
        result.ShouldHaveValidationErrorFor(x => x)
              .WithErrorMessage("Metoda plaćanja mora biti prazna dok sve stavke nisu poslužene. Kada su sve stavke poslužene, za iznose do 50 može se platiti samo gotovinom.");
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