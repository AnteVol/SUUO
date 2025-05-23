using SUUO_DZ3.Data;
using SUUO_DZ3.Models;
using SUUO_DZ3.Models.Enums;
using SUUO_DZ3.Models.Validation;
using SUUO_DZ3.Tests.Helpers;

namespace SUUO_DZ3.Tests.Unit_tests.Podatkovni_sloj;

public class DataTests
{
    private readonly ApplicationDbContext _context;
    private readonly NarudzbaValidator _narudzbaValidator;
    private readonly KonobarValidator _konobarValidator;
    private readonly KuharValidator _kuharValidator;

    public DataTests()
    {
        _context = TestDbContextFactory.Create();
        _narudzbaValidator = new NarudzbaValidator();
        _konobarValidator = new KonobarValidator();
        _kuharValidator = new KuharValidator();
    }

    [Fact]
    public async Task DodavanjeKonobara_Uspjesno()
    {
        var konobar = new Konobar
        {
            IdKonobar = Guid.NewGuid(),
            Ime = "Marko",
            Prezime = "Marković",
            Telefon = "+385-98-1234567",
            Email = "marko.markovic@restoran.com"
        };

        var validacija = _konobarValidator.Validate(konobar);
        Assert.True(validacija.IsValid, string.Join(", ", validacija.Errors.Select(e => e.ErrorMessage)));

        await _context.Konobari.AddAsync(konobar);
        await _context.SaveChangesAsync();

        var konobarIzBaze = await _context.Konobari.FindAsync(konobar.IdKonobar);
        Assert.NotNull(konobarIzBaze);
        Assert.Equal("Marko", konobarIzBaze.Ime);
        Assert.Equal("marko.markovic@restoran.com", konobarIzBaze.Email);
    }
    
    public async Task DodavanjeKonobara_NeUspjesno_NeispravanEmail()
    {
        var konobar = new Konobar
        {
            IdKonobar = Guid.NewGuid(),
            Ime = "Petra",
            Prezime = "Petrić",
            Telefon = "+385-98-7654321",
            Email = "neispravanemail"
        };

        var validacija = _konobarValidator.Validate(konobar);
        Assert.False(validacija.IsValid);

        if (validacija.IsValid)
        {
            await _context.Konobari.AddAsync(konobar);
            await _context.SaveChangesAsync();
        }

        var konobarIzBaze = await _context.Konobari.FindAsync(konobar.IdKonobar);
        Assert.Null(konobarIzBaze);
    }
    
    [Fact]
    public async Task DodavanjeNarudzbe_Uspjesno()
    {
        var narudzba = new Narudzba
        {
            NarudzbaId = Guid.NewGuid(),
            Stol = "Stol01",
            VrijemeNarudzbe = DateTime.Now
        };

        var rezultatValidacije = _narudzbaValidator.Validate(narudzba);
        
        if (rezultatValidacije.IsValid)
        {
            await _context.Narudzbe.AddAsync(narudzba);
            await _context.SaveChangesAsync();
        }

        var narudzbaBaza = await _context.Narudzbe.FindAsync(narudzba.NarudzbaId);
        Assert.NotNull(narudzbaBaza);
        Assert.Equal("Stol01", (string) narudzbaBaza.Stol);
    }
    
    [Fact]
    public async Task DodavanjeNarudzbe_NeUspjesno_NeispravanStol()
    {
        var narudzba = new Narudzba
        {
            NarudzbaId = Guid.NewGuid(),
            Stol = "Stol121",
            VrijemeNarudzbe = DateTime.Now
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
    public async Task DodavanjeKompletneNarudzbe_Uspjesno()
    {
        var konobar = new Konobar
        {
            IdKonobar = Guid.NewGuid(),
            Ime = "Ivan",
            Prezime = "Ivić",
            Telefon = "+385-91-1234567",
            Email = "ivan.konobar@test.com"
        };

        var kuhar = new Kuhar
        {
            IdKuhar = Guid.NewGuid(),
            Ime = "Ana",
            Prezime = "Anić",
            Telefon = "+385-91-7654321",
            Email = "ana.kuhar@test.com",
            Specijaliteti = new List<string> { "Pizza", "Tjestenina" }
        };

        var narudzba = new Narudzba
        {
            NarudzbaId = Guid.NewGuid(),
            VrijemeNarudzbe = new DateTime(2025, 10, 1, 12, 30, 0),
            Stol = "Stol05",
            Status = StatusNarudzbe.UPripremi,
            KonobarId = konobar.IdKonobar,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe
                {
                    StavkaNarudzbeId = Guid.NewGuid(),
                    Naziv = "Pizza Margherita",
                    Kolicina = 2,
                    Cijena = 30,
                    AkcijskaPonuda = false,
                    Status = StatusStavke.NaCekanju
                },
                new StavkaNarudzbe
                {
                    StavkaNarudzbeId = Guid.NewGuid(),
                    Naziv = "Coca Cola",
                    Kolicina = 1,
                    Cijena = 15,
                    AkcijskaPonuda = true,
                    Status = StatusStavke.NaCekanju
                }
            }
        };

        var validacija = _narudzbaValidator.Validate(narudzba);
        Assert.True(validacija.IsValid, string.Join(", ", validacija.Errors.Select(e => e.ErrorMessage)));
        
        await _context.Konobari.AddAsync(konobar);
        await _context.Kuhari.AddAsync(kuhar);
        await _context.Narudzbe.AddAsync(narudzba);
        await _context.SaveChangesAsync();
        
        var narudzbaIzBaze = await _context.Narudzbe.FindAsync(narudzba.NarudzbaId);
        Assert.NotNull(narudzbaIzBaze);
        Assert.Equal("Stol05", narudzbaIzBaze.Stol);
        Assert.Equal(2, narudzba.StavkeNarudzbi.Count);
    }
    
    
    public void Dispose()
    {
        _context.Dispose();
    }
}