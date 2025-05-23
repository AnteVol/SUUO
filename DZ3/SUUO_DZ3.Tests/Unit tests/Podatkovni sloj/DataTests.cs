using SUUO_DZ3.Data;
using SUUO_DZ3.Models;
using SUUO_DZ3.Models.Enums;
using SUUO_DZ3.Models.Validation;
using SUUO_DZ3.Tests.Helpers;

namespace SUUO_DZ3.Tests.Unit_tests.Podatkovni_sloj;

public class DataTestovi
{
    private readonly ApplicationDbContext _context;
    private readonly NarudzbaValidator _narudzbaValidator;
    private readonly KonobarValidator _konobarValidator;
    private readonly KuharValidator _kuharValidator;
    private readonly StavkaNarudbeValidator _stavkaValidator;

    public DataTestovi()
    {
        _context = TestDbContextFactory.Create();
        _narudzbaValidator = new NarudzbaValidator();
        _konobarValidator = new KonobarValidator();
        _kuharValidator = new KuharValidator();
        _stavkaValidator = new StavkaNarudbeValidator();
    }

    [Fact]
    public async Task DodavanjeKonobara_Uspjesno()
    {
        var konobar = new Konobar
        {
            IdKonobar = Guid.NewGuid(),
            Ime = "Marko",
            Prezime = "Marković",
            Telefon = "+385-912345678",
            Email = "marko.markovic@restaurant.com"
        };

        var rezultatValidacije = _konobarValidator.Validate(konobar);
        
        if (rezultatValidacije.IsValid)
        {
            await _context.Konobari.AddAsync(konobar);
            await _context.SaveChangesAsync();
        }

        var konobarBaza = await _context.Konobari.FindAsync(konobar.IdKonobar);
        Assert.NotNull(konobarBaza);
        Assert.Equal("Marko", konobarBaza.Ime);
        Assert.Equal("marko.markovic@restaurant.com", konobarBaza.Email);
    }

    [Fact]
    public async Task DodavanjeKonobara_NeUspjesno_NeispravanTelefon()
    {
        var konobar = new Konobar
        {
            IdKonobar = Guid.NewGuid(),
            Ime = "Petra",
            Prezime = "Petrić",
            Telefon = "091-1234567",
            Email = "petra.petric@restaurant.com"
        };

        var rezultatValidacije = _konobarValidator.Validate(konobar);
        Assert.False(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.Konobari.AddAsync(konobar);
            await _context.SaveChangesAsync();
        }

        var konobarBaza = await _context.Konobari.FindAsync(konobar.IdKonobar);
        Assert.Null(konobarBaza);
    }

    [Fact]
    public async Task DodavanjeKonobara_NeUspjesno_NeispravanEmail()
    {
        var konobar = new Konobar
        {
            IdKonobar = Guid.NewGuid(),
            Ime = "Ana",
            Prezime = "Anić",
            Telefon = "+385-987654321",
            Email = "neispravanemail.bez.at"
        };

        var rezultatValidacije = _konobarValidator.Validate(konobar);
        Assert.False(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.Konobari.AddAsync(konobar);
            await _context.SaveChangesAsync();
        }

        var konobarBaza = await _context.Konobari.FindAsync(konobar.IdKonobar);
        Assert.Null(konobarBaza);
    }

    [Fact]
    public async Task DodavanjeKuhara_Uspjesno()
    {
        var kuhar = new Kuhar
        {
            IdKuhar = Guid.NewGuid(),
            Ime = "Stefan",
            Prezime = "Stefanović",
            Telefon = "+385-925555555",
            Email = "stefan.kuhar@restaurant.com",
            Specijaliteti = new List<string> { "Grill", "Salate", "Deserti" }
        };

        var rezultatValidacije = _kuharValidator.Validate(kuhar);
        
        if (rezultatValidacije.IsValid)
        {
            await _context.Kuhari.AddAsync(kuhar);
            await _context.SaveChangesAsync();
        }

        var kuharBaza = await _context.Kuhari.FindAsync(kuhar.IdKuhar);
        Assert.NotNull(kuharBaza);
        Assert.Equal("Stefan", kuharBaza.Ime);
        Assert.Equal(3, kuharBaza.Specijaliteti.Count);
        Assert.Contains("Grill", kuharBaza.Specijaliteti);
    }

    [Fact]
    public async Task DodavanjeKuhara_NeUspjesno_BezSpecijaliteta()
    {
        var kuhar = new Kuhar
        {
            IdKuhar = Guid.NewGuid(),
            Ime = "Luka",
            Prezime = "Lukić",
            Telefon = "+385-951111111",
            Email = "luka.kuhar@restaurant.com",
            Specijaliteti = new List<string>()
        };

        var rezultatValidacije = _kuharValidator.Validate(kuhar);
        Assert.False(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.Kuhari.AddAsync(kuhar);
            await _context.SaveChangesAsync();
        }

        var kuharBaza = await _context.Kuhari.FindAsync(kuhar.IdKuhar);
        Assert.Null(kuharBaza);
    }

    [Fact]
    public async Task DodavanjeKuhara_NeUspjesno_NeispravanTelefon()
    {
        var kuhar = new Kuhar
        {
            IdKuhar = Guid.NewGuid(),
            Ime = "Marija",
            Prezime = "Marić",
            Telefon = "385-92-1111111",
            Email = "marija.kuhar@restaurant.com",
            Specijaliteti = new List<string> { "Pizza" }
        };

        var rezultatValidacije = _kuharValidator.Validate(kuhar);
        Assert.False(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.Kuhari.AddAsync(kuhar);
            await _context.SaveChangesAsync();
        }

        var kuharBaza = await _context.Kuhari.FindAsync(kuhar.IdKuhar);
        Assert.Null(kuharBaza);
    }

    [Fact]
    public async Task DodavanjeStavkeNarudzbe_Uspjesno()
    {
        var stavka = new StavkaNarudzbe
        {
            StavkaNarudzbeId = Guid.NewGuid(),
            NarudzbaId = Guid.NewGuid(),
            Naziv = "Pileći odrezak",
            Kolicina = 1,
            Cijena = 45.50m,
            AkcijskaPonuda = false,
            Status = StatusStavke.NaCekanju
        };

        var rezultatValidacije = _stavkaValidator.Validate(stavka);
        
        if (rezultatValidacije.IsValid)
        {
            await _context.StavkeNarudzbe.AddAsync(stavka);
            await _context.SaveChangesAsync();
        }

        var stavkaBaza = await _context.StavkeNarudzbe.FindAsync(stavka.StavkaNarudzbeId);
        Assert.NotNull(stavkaBaza);
        Assert.Equal("Pileći odrezak", stavkaBaza.Naziv);
        Assert.Equal(45.50m, stavkaBaza.Cijena);
    }

    [Fact]
    public async Task DodavanjeStavkeNarudzbe_NeUspjesno_NulaCijena()
    {
        var stavka = new StavkaNarudzbe
        {
            StavkaNarudzbeId = Guid.NewGuid(),
            NarudzbaId = Guid.NewGuid(),
            Naziv = "Juha od rajčice",
            Kolicina = 1,
            Cijena = 0,
            AkcijskaPonuda = false,
            Status = StatusStavke.NaCekanju
        };

        var rezultatValidacije = _stavkaValidator.Validate(stavka);
        Assert.False(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.StavkeNarudzbe.AddAsync(stavka);
            await _context.SaveChangesAsync();
        }

        var stavkaBaza = await _context.StavkeNarudzbe.FindAsync(stavka.StavkaNarudzbeId);
        Assert.Null(stavkaBaza);
    }

    [Fact]
    public async Task DodavanjeStavkeNarudzbe_AkcijskaPonuda_Uspjesno()
    {
        var stavka = new StavkaNarudzbe
        {
            StavkaNarudzbeId = Guid.NewGuid(),
            NarudzbaId = Guid.NewGuid(),
            Naziv = "Coca Cola",
            Kolicina = 3,
            Cijena = 8.50m,
            AkcijskaPonuda = true,
            Status = StatusStavke.NaCekanju
        };

        var rezultatValidacije = _stavkaValidator.Validate(stavka);
        Assert.True(rezultatValidacije.IsValid);
        
        if (rezultatValidacije.IsValid)
        {
            await _context.StavkeNarudzbe.AddAsync(stavka);
            await _context.SaveChangesAsync();
        }

        var stavkaBaza = await _context.StavkeNarudzbe.FindAsync(stavka.StavkaNarudzbeId);
        Assert.NotNull(stavkaBaza);
        Assert.True(stavkaBaza.AkcijskaPonuda);
    }

    [Fact]
    public async Task DodavanjeStavkeNarudzbe_AkcijskaPonuda_NeUspjesno_PrevisokaCijena()
    {
        var stavka = new StavkaNarudzbe
        {
            StavkaNarudzbeId = Guid.NewGuid(),
            NarudzbaId = Guid.NewGuid(),
            Naziv = "Skupa Pizza",
            Kolicina = 2,
            Cijena = 15.00m,
            AkcijskaPonuda = true,
            Status = StatusStavke.NaCekanju
        };

        var rezultatValidacije = _stavkaValidator.Validate(stavka);
        Assert.False(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.StavkeNarudzbe.AddAsync(stavka);
            await _context.SaveChangesAsync();
        }

        var stavkaBaza = await _context.StavkeNarudzbe.FindAsync(stavka.StavkaNarudzbeId);
        Assert.Null(stavkaBaza);
    }

    [Fact]
    public async Task DodavanjeStavkeNarudzbe_AkcijskaPonuda_NeUspjesno_PrevisokaKolicina()
    {
        var stavka = new StavkaNarudzbe
        {
            StavkaNarudzbeId = Guid.NewGuid(),
            NarudzbaId = Guid.NewGuid(),
            Naziv = "Pivo",
            Kolicina = 10,
            Cijena = 5.00m,
            AkcijskaPonuda = true,
            Status = StatusStavke.NaCekanju
        };

        var rezultatValidacije = _stavkaValidator.Validate(stavka);
        Assert.False(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.StavkeNarudzbe.AddAsync(stavka);
            await _context.SaveChangesAsync();
        }

        var stavkaBaza = await _context.StavkeNarudzbe.FindAsync(stavka.StavkaNarudzbeId);
        Assert.Null(stavkaBaza);
    }

    [Fact]
    public async Task DodavanjeNarudzbe_NeUspjesno_IzvanRadnogVremena()
    {
        var narudzba = new Narudzba
        {
            NarudzbaId = Guid.NewGuid(),
            Stol = "Stol03",
            VrijemeNarudzbe = new DateTime(2025, 1, 15, 9, 30, 0)
        };

        var rezultatValidacije = _narudzbaValidator.Validate(narudzba);
        Assert.False(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.Narudzbe.AddAsync(narudzba);
            await _context.SaveChangesAsync();
        }

        var narudzbaBaza = await _context.Narudzbe.FindAsync(narudzba.NarudzbaId);
        Assert.Null(narudzbaBaza);
    }

    [Fact]
    public async Task DodavanjeNarudzbe_NeUspjesno_NeispravanStol()
    {
        var narudzba = new Narudzba
        {
            NarudzbaId = Guid.NewGuid(),
            Stol = "Stol26",
            VrijemeNarudzbe = new DateTime(2025, 1, 15, 15, 0, 0)
        };

        var rezultatValidacije = _narudzbaValidator.Validate(narudzba);
        Assert.False(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.Narudzbe.AddAsync(narudzba);
            await _context.SaveChangesAsync();
        }

        var narudzbaBaza = await _context.Narudzbe.FindAsync(narudzba.NarudzbaId);
        Assert.Null(narudzbaBaza);
    }

    [Fact]
    public async Task DodavanjeNarudzbe_URadnoVrijeme_Uspjesno()
    {
        var narudzba = new Narudzba
        {
            NarudzbaId = Guid.NewGuid(),
            Stol = "Stol15",
            VrijemeNarudzbe = new DateTime(2025, 1, 15, 18, 30, 0),
            Status = StatusNarudzbe.Naplaceno,
            MetodaPlacanja = MetodaPlacanja.Gotovina,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe
                {
                    StavkaNarudzbeId = Guid.NewGuid(),
                    Naziv = "Kava",
                    Kolicina = 2,
                    Cijena = 12,
                    Status = StatusStavke.Pripremljeno
                }
            }
        };

        var rezultatValidacije = _narudzbaValidator.Validate(narudzba);
        Assert.True(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.Narudzbe.AddAsync(narudzba);
            await _context.SaveChangesAsync();
        }

        var narudzbaBaza = await _context.Narudzbe.FindAsync(narudzba.NarudzbaId);
        Assert.NotNull(narudzbaBaza);
        Assert.Equal("Stol15", narudzbaBaza.Stol);
    }

    [Fact]
    public async Task NarudzbaMetodaPlacanja_MalaCijena_MoraGotovina()
    {
        var narudzba = new Narudzba
        {
            NarudzbaId = Guid.NewGuid(),
            Stol = "Stol10",
            VrijemeNarudzbe = new DateTime(2025, 1, 15, 12, 0, 0),
            MetodaPlacanja = MetodaPlacanja.Kartica,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe
                {
                    StavkaNarudzbeId = Guid.NewGuid(),
                    Naziv = "Kava",
                    Kolicina = 2,
                    Cijena = 12.00m
                }
            }
        };

        var rezultatValidacije = _narudzbaValidator.Validate(narudzba);
        Assert.False(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.Narudzbe.AddAsync(narudzba);
            await _context.SaveChangesAsync();
        }

        var narudzbaBaza = await _context.Narudzbe.FindAsync(narudzba.NarudzbaId);
        Assert.Null(narudzbaBaza);
    }

    [Fact]
    public async Task StatusNarudzbe_NePripremljeneStavke_NeMožePoslužiti()
    {
        var narudzba = new Narudzba
        {
            NarudzbaId = Guid.NewGuid(),
            Stol = "Stol08",
            VrijemeNarudzbe = new DateTime(2025, 1, 15, 14, 0, 0),
            Status = StatusNarudzbe.Posluzeno, 
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe
                {
                    StavkaNarudzbeId = Guid.NewGuid(),
                    Naziv = "Pizza",
                    Kolicina = 1,
                    Cijena = 35.00m,
                    Status = StatusStavke.UPripremi
                }
            }
        };

        var rezultatValidacije = _narudzbaValidator.Validate(narudzba);
        Assert.False(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.Narudzbe.AddAsync(narudzba);
            await _context.SaveChangesAsync();
        }

        var narudzbaBaza = await _context.Narudzbe.FindAsync(narudzba.NarudzbaId);
        Assert.Null(narudzbaBaza);
    }

    [Fact]
    public async Task KompletnaNarudzba_SvePripremljeno_MožePoslužiti()
    {
        var narudzba = new Narudzba
        {
            NarudzbaId = Guid.NewGuid(),
            Stol = "Stol12",
            VrijemeNarudzbe = new DateTime(2025, 1, 15, 19, 0, 0),
            Status = StatusNarudzbe.Posluzeno,
            MetodaPlacanja = MetodaPlacanja.Gotovina,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe
                {
                    StavkaNarudzbeId = Guid.NewGuid(),
                    Naziv = "Pasta",
                    Kolicina = 1,
                    Cijena = 28.00m,
                    Status = StatusStavke.Pripremljeno
                },
                new StavkaNarudzbe
                {
                    StavkaNarudzbeId = Guid.NewGuid(),
                    Naziv = "Salata",
                    Kolicina = 1,
                    Cijena = 18.00m,
                    Status = StatusStavke.Pripremljeno 
                }
            }
        };

        var rezultatValidacije = _narudzbaValidator.Validate(narudzba);
        Assert.True(rezultatValidacije.IsValid);

        if (rezultatValidacije.IsValid)
        {
            await _context.Narudzbe.AddAsync(narudzba);
            await _context.SaveChangesAsync();
        }

        var narudzbaBaza = await _context.Narudzbe.FindAsync(narudzba.NarudzbaId);
        Assert.NotNull(narudzbaBaza);
        Assert.Equal(StatusNarudzbe.Posluzeno, narudzbaBaza.Status);
        Assert.Equal(2, narudzbaBaza.StavkeNarudzbi.Count);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}