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
    public async Task KonobarController_CreateAndGet_ShouldWork()
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
    public async Task KonobarController_CreateWithInvalidData_ShouldReturnBadRequest()
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
    public async Task NarudzbaController_CompleteWorkflow_ShouldWork()
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
    public async Task DohvacanjeNepostojecegKonobara_TrebaVratit404()
    {
        var response = await _client.GetAsync($"/api/konobar/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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