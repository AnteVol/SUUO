using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SUUO_DZ3.Data;
using SUUO_DZ3.Models;
using SUUO_DZ3.Models.Enums;

namespace SUUO_DZ3.Tests.Integration_tests;

public class SUUOIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceScopeFactory _scopeFactory;

    public SUUOIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var dbName = Guid.NewGuid().ToString();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase(dbName); });
            });
        });

        _client = _factory.CreateClient();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task KonobarController_KreirajIDohvati_Radi()
    {
        var konobar = new Konobar
        {
            Ime = "Marko",
            Prezime = "Petrović",
            Telefon = "+385-912345678",
            Email = "marko.petrovic@email.com",
            Aktivan = true
        };

        var json = JsonSerializer.Serialize(konobar, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var createResponse = await _client.PostAsync("/api/konobar", content);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var kreiraniKonobar = JsonSerializer.Deserialize<Konobar>(
            await createResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotEqual(Guid.Empty, kreiraniKonobar?.IdKonobar);
        Assert.Equal("Marko", kreiraniKonobar?.Ime);

        var getResponse = await _client.GetAsync($"/api/konobar/{kreiraniKonobar?.IdKonobar}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var dohvaceniKonobar = JsonSerializer.Deserialize<Konobar>(
            await getResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.Equal(kreiraniKonobar?.IdKonobar, dohvaceniKonobar?.IdKonobar);
        Assert.Equal("Marko", dohvaceniKonobar?.Ime);
        Assert.Equal("+385-912345678", dohvaceniKonobar?.Telefon);
    }

    [Fact]
    public async Task KonobarController_KreirajSValidacijskimPogreskama_ReturnBadRequest()
    {
        var neispravanKonobar = new Konobar
        {
            Ime = "Ana",
            Prezime = "Marić",
            Telefon = "123456789",
            Email = "invalid-email",
            Aktivan = true
        };

        var json = JsonSerializer.Serialize(neispravanKonobar, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/konobar", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task NarudzbaController_CjelokupniProcesKreiranja_Radi()
    {
        var konobar = await KreirajKonobara();

        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2024, 12, 15, 14, 30, 0),
            KonobarId = konobar.IdKonobar,
            Stol = "Stol08",
            Status = StatusNarudzbe.Zaprimljeno,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe
                {
                    Naziv = "Pizza Quattro Stagioni",
                    Kolicina = 1,
                    Cijena = 28.50m,
                    AkcijskaPonuda = false,
                    Status = StatusStavke.NaCekanju
                }
            }
        };

        var json = JsonSerializer.Serialize(narudzba, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var createResponse = await _client.PostAsync("/api/narudzbe", content);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var kreiranaNarudzba = JsonSerializer.Deserialize<Narudzba>(
            await createResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });

        Assert.NotEqual(Guid.Empty, kreiranaNarudzba?.NarudzbaId);
        Assert.Equal("Stol08", kreiranaNarudzba?.Stol);
        Assert.Equal(28.50m, kreiranaNarudzba?.UkupnaCijena);

        var getResponse = await _client.GetAsync($"/api/narudzbe/{kreiranaNarudzba?.NarudzbaId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var dohvacenaNarudzba = JsonSerializer.Deserialize<Narudzba>(
            await getResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });
        ;

        Assert.Equal(kreiranaNarudzba?.NarudzbaId, dohvacenaNarudzba?.NarudzbaId);
        Assert.NotNull(dohvacenaNarudzba?.StavkeNarudzbi);
        Assert.Single(dohvacenaNarudzba.StavkeNarudzbi);
    }

    [Fact]
    public async Task DohvacanjeNepostojecegKonobara_BadRequest()
    {
        var response = await _client.GetAsync($"/api/konobar/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task StavkaNarudzbeController_KreirajIDohvati_Radi()
    {
        var konobar = await KreirajKonobara();
        var narudzba = await KreirajNarudzbu(konobar.IdKonobar);

        var stavka = new StavkaNarudzbe
        {
            NarudzbaId = narudzba.NarudzbaId,
            Naziv = "Pizza Margherita",
            Kolicina = 2,
            Cijena = 25.00m,
            AkcijskaPonuda = false,
            Status = StatusStavke.NaCekanju
        };

        var json = JsonSerializer.Serialize(stavka, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var createResponse = await _client.PostAsync("/api/stavkanarudzbe", content);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var kreirana = JsonSerializer.Deserialize<StavkaNarudzbe>(
            await createResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });

        Assert.NotEqual(Guid.Empty, kreirana?.StavkaNarudzbeId);
        Assert.Equal("Pizza Margherita", kreirana?.Naziv);
        Assert.Equal(25.00m, kreirana?.Cijena);

        var getResponse = await _client.GetAsync($"/api/stavkanarudzbe/{kreirana?.StavkaNarudzbeId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var dohvacena = JsonSerializer.Deserialize<StavkaNarudzbe>(
            await getResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });

        Assert.Equal(kreirana?.StavkaNarudzbeId, dohvacena?.StavkaNarudzbeId);
        Assert.Equal("Pizza Margherita", dohvacena?.Naziv);
    }

    [Fact]
    public async Task StavkaNarudzbeController_KreirajSAkcijskomPonudom_Radi()
    {
        var konobar = await KreirajKonobara();
        var narudzba = await KreirajNarudzbu(konobar.IdKonobar);

        var akcijskaStavka = new StavkaNarudzbe
        {
            NarudzbaId = narudzba.NarudzbaId,
            Naziv = "Akcijska Pizza",
            Kolicina = 3, 
            Cijena = 8.50m,
            AkcijskaPonuda = true,
            Status = StatusStavke.NaCekanju
        };

        var json = JsonSerializer.Serialize(akcijskaStavka, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/stavkanarudzbe", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task StavkaNarudzbeController_KreirajSNeispravnomAkcijskomPonudom_ReturnBadRequest()
    {
        var konobar = await KreirajKonobara();
        var narudzba = await KreirajNarudzbu(konobar.IdKonobar);
        
        var neispravanaCijena = new StavkaNarudzbe
        {
            NarudzbaId = narudzba.NarudzbaId,
            Naziv = "Skupa akcijska pizza",
            Kolicina = 2,
            Cijena = 15.00m,
            AkcijskaPonuda = true,
            Status = StatusStavke.NaCekanju
        };

        var json1 = JsonSerializer.Serialize(neispravanaCijena, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });
        var content1 = new StringContent(json1, Encoding.UTF8, "application/json");

        var response1 = await _client.PostAsync("/api/stavkanarudzbe", content1);
        Assert.Equal(HttpStatusCode.BadRequest, response1.StatusCode);

        var neispravaKolicina = new StavkaNarudzbe
        {
            NarudzbaId = narudzba.NarudzbaId,
            Naziv = "Akcijska pizza",
            Kolicina = 6,
            Cijena = 8.00m,
            AkcijskaPonuda = true,
            Status = StatusStavke.NaCekanju
        };

        var json2 = JsonSerializer.Serialize(neispravaKolicina, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });
        var content2 = new StringContent(json2, Encoding.UTF8, "application/json");

        var response2 = await _client.PostAsync("/api/stavkanarudzbe", content2);
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
    }

    [Fact]
    public async Task StavkaNarudzbeController_KreirajBezCijene_ReturnBadRequest()
    {
        var konobar = await KreirajKonobara();
        var narudzba = await KreirajNarudzbu(konobar.IdKonobar);

        var stavka = new StavkaNarudzbe
        {
            NarudzbaId = narudzba.NarudzbaId,
            Naziv = "Besplatna pizza",
            Kolicina = 1,
            Cijena = 0m,
            AkcijskaPonuda = false,
            Status = StatusStavke.NaCekanju
        };

        var json = JsonSerializer.Serialize(stavka, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/stavkanarudzbe", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task StavkaNarudzbeController_DohvatiPoNarudzbaId_Radi()
    {
        var konobar = await KreirajKonobara();
        var narudzba = await KreirajNarudzbu(konobar.IdKonobar);

        await KreirajStavku(narudzba.NarudzbaId, "Pizza 1", 15.00m);
        await KreirajStavku(narudzba.NarudzbaId, "Pizza 2", 20.00m);

        var response = await _client.GetAsync($"/api/stavkanarudzbe/narudzba/{narudzba.NarudzbaId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stavke = JsonSerializer.Deserialize<List<StavkaNarudzbe>>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });

        Assert.NotNull(stavke);
        Assert.Equal(2, stavke.Count);
        Assert.All(stavke, s => Assert.Equal(narudzba.NarudzbaId, s.NarudzbaId));
    }

    [Fact]
    public async Task StavkaNarudzbeController_Azuriranje_Radi()
    {
        var konobar = await KreirajKonobara();
        var narudzba = await KreirajNarudzbu(konobar.IdKonobar);
        var stavka = await KreirajStavku(narudzba.NarudzbaId, "Originalna Pizza", 20.00m);

        stavka.Naziv = "Ažurirana Pizza";
        stavka.Cijena = 25.00m;
        stavka.Status = StatusStavke.Pripremljeno;

        var json = JsonSerializer.Serialize(stavka, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/stavkanarudzbe/{stavka.StavkaNarudzbeId}", content);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/stavkanarudzbe/{stavka.StavkaNarudzbeId}");
        var azurirana = JsonSerializer.Deserialize<StavkaNarudzbe>(
            await getResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });

        Assert.Equal("Ažurirana Pizza", azurirana?.Naziv);
        Assert.Equal(25.00m, azurirana?.Cijena);
        Assert.Equal(StatusStavke.Pripremljeno, azurirana?.Status);
    }

    [Fact]
    public async Task StavkaNarudzbeController_Brisanje_Radi()
    {
        var konobar = await KreirajKonobara();
        var narudzba = await KreirajNarudzbu(konobar.IdKonobar);
        var stavka = await KreirajStavku(narudzba.NarudzbaId, "Pizza za brisanje", 15.00m);

        var deleteResponse = await _client.DeleteAsync($"/api/stavkanarudzbe/{stavka.StavkaNarudzbeId}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/stavkanarudzbe/{stavka.StavkaNarudzbeId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task NarudzbaController_KreirajSIspravnimStolom_Radi()
    {
        var konobar = await KreirajKonobara();

        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2024, 12, 15, 15, 30, 0),
            KonobarId = konobar.IdKonobar,
            Stol = "Stol12",
            Status = StatusNarudzbe.Zaprimljeno
        };

        var json = JsonSerializer.Serialize(narudzba, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/narudzbe", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task NarudzbaController_KreirajVanRadnogVremena_ReturnBadRequest()
    {
        var konobar = await KreirajKonobara();

        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2024, 12, 15, 9, 30, 0),
            KonobarId = konobar.IdKonobar,
            Stol = "Stol12",
            Status = StatusNarudzbe.Zaprimljeno
        };

        var json = JsonSerializer.Serialize(narudzba, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/narudzbe", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task NarudzbaController_KreirajSNeispravnimStolom_ReturnBadRequest()
    {
        var konobar = await KreirajKonobara();

        var neispravanFormati = new[] { "Stol26", "Stol999", "stol05", "Table05", "Stol5", "" };

        foreach (var stolFormat in neispravanFormati)
        {
            var narudzba = new Narudzba
            {
                VrijemeNarudzbe = new DateTime(2024, 12, 15, 15, 30, 0),
                KonobarId = konobar.IdKonobar,
                Stol = stolFormat,
                Status = StatusNarudzbe.Zaprimljeno
            };

            var json = JsonSerializer.Serialize(narudzba, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/narudzbe", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    [Fact]
    public async Task NarudzbaController_NeispravanNacinPlacanja_CijenaIspod50_TrebaTrazitGotovinu()
    {
        var konobar = await KreirajKonobara();

        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2024, 12, 15, 15, 30, 0),
            KonobarId = konobar.IdKonobar,
            Stol = "Stol05",
            Status = StatusNarudzbe.Zaprimljeno,
            StavkeNarudzbi = new List<StavkaNarudzbe>
            {
                new StavkaNarudzbe
                {
                    Naziv = "Jeftina pizza",
                    Kolicina = 1,
                    Cijena = 30.00m,
                    AkcijskaPonuda = false,
                    Status = StatusStavke.Pripremljeno 
                }
            },
            MetodaPlacanja = MetodaPlacanja.Gotovina
        };

        var json = JsonSerializer.Serialize(narudzba, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/narudzbe", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task NarudzbaController_DohvatiSve_Radi()
    {
        var konobar = await KreirajKonobara();
        await KreirajNarudzbu(konobar.IdKonobar);
        await KreirajNarudzbu(konobar.IdKonobar);

        var response = await _client.GetAsync("/api/narudzbe");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var narudzbe = JsonSerializer.Deserialize<List<Narudzba>>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });

        Assert.NotNull(narudzbe);
        Assert.True(narudzbe.Count >= 2);
    }

    [Fact]
    public async Task NarudzbaController_Brisanje_Radi()
    {
        var konobar = await KreirajKonobara();
        var narudzba = await KreirajNarudzbu(konobar.IdKonobar);

        var deleteResponse = await _client.DeleteAsync($"/api/narudzbe/{narudzba.NarudzbaId}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/narudzbe/{narudzba.NarudzbaId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DohvacanjeNepostojeceStavke_BadRequest()
    {
        var response = await _client.GetAsync($"/api/stavkanarudzbe/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DohvacanjeNepostojeceNarudzbe_BadRequest()
    {
        var response = await _client.GetAsync($"/api/narudzbe/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<Narudzba> KreirajNarudzbu(Guid konobarId)
    {
        var narudzba = new Narudzba
        {
            VrijemeNarudzbe = new DateTime(2024, 12, 15, 15, 30, 0),
            KonobarId = konobarId,
            Stol = "Stol08",
            Status = StatusNarudzbe.Zaprimljeno
        };

        var json = JsonSerializer.Serialize(narudzba, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/narudzbe", content);

        var kreirana = JsonSerializer.Deserialize<Narudzba>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });

        return kreirana;
    }

    private async Task<StavkaNarudzbe> KreirajStavku(Guid narudzbaId, string naziv, decimal cijena)
    {
        var stavka = new StavkaNarudzbe
        {
            NarudzbaId = narudzbaId,
            Naziv = naziv,
            Kolicina = 1,
            Cijena = cijena,
            AkcijskaPonuda = false,
            Status = StatusStavke.NaCekanju
        };

        var json = JsonSerializer.Serialize(stavka, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/stavkanarudzbe", content);

        var kreirana = JsonSerializer.Deserialize<StavkaNarudzbe>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });

        return kreirana;
    }

    private async Task<Konobar> KreirajKonobara()
    {
        var konobar = new Konobar
        {
            Ime = "TestKonobar",
            Prezime = "TestPrezime",
            Telefon = "+385-987654321",
            Email = "test@example.com",
            Aktivan = true
        };

        var json = JsonSerializer.Serialize(konobar, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/konobar", content);

        var kreiraniKonobar = JsonSerializer.Deserialize<Konobar>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return kreiraniKonobar;
    }
}