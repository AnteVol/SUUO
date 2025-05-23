using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SUUO_DZ3.Data;
using SUUO_DZ3.Models;
using SUUO_DZ3.Models.Enums;

namespace SUUO_DZ3.Tests.Integration_tests;

public class RestaurantApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceScopeFactory _scopeFactory;

    public RestaurantApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var dbName = Guid.NewGuid().ToString(); // ➜ izolirana baza

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
            });
        });

        _client = _factory.CreateClient();

        // ➜ koristimo za SeedTestData()
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

        var createdKonobar = JsonSerializer.Deserialize<Konobar>(
            await createResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotEqual(Guid.Empty, createdKonobar.IdKonobar);
        Assert.Equal("Marko", createdKonobar.Ime);
        
        var getResponse = await _client.GetAsync($"/api/konobar/{createdKonobar.IdKonobar}");
        
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedKonobar = JsonSerializer.Deserialize<Konobar>(
            await getResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.Equal(createdKonobar.IdKonobar, retrievedKonobar.IdKonobar);
        Assert.Equal("Marko", retrievedKonobar.Ime);
        Assert.Equal("+385-912345678", retrievedKonobar.Telefon);
    }

    [Fact]
    public async Task KonobarController_CreateWithInvalidData_ShouldReturnBadRequest()
    {
        var invalidKonobar = new Konobar
        {
            Ime = "Ana",
            Prezime = "Marić",
            Telefon = "123456789",
            Email = "invalid-email",
            Aktivan = true
        };

        var json = JsonSerializer.Serialize(invalidKonobar, new JsonSerializerOptions
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
        var konobar = await CreateKonobar();

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

        var responseJson = await createResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Full JSON: {responseJson}");

// Test just the stavke array first
        var document = JsonDocument.Parse(responseJson);
        var stavkeElement = document.RootElement.GetProperty("stavkeNarudzbi");
        var stavkeJson = stavkeElement.GetRawText();
        Console.WriteLine($"Stavke JSON: {stavkeJson}");

// Try to deserialize just the array
        try
        {
            var stavkeList = JsonSerializer.Deserialize<List<StavkaNarudzbe>>(
                stavkeJson, 
                new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });
            Console.WriteLine($"Stavke deserialized successfully: {stavkeList.Count} items");
            Console.WriteLine($"First item: {stavkeList[0].Naziv}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Stavke deserialization failed: {ex.Message}");
            Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
        }
        
        var createdNarudzba = JsonSerializer.Deserialize<Narudzba>(
            await createResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotEqual(Guid.Empty, createdNarudzba.NarudzbaId);
        Assert.Equal("Stol08", createdNarudzba.Stol);
        Assert.Equal(28.50m, createdNarudzba.UkupnaCijena);
        
        var getResponse = await _client.GetAsync($"/api/narudzbe/{createdNarudzba.NarudzbaId}");
        
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedNarudzba = JsonSerializer.Deserialize<Narudzba>(
            await getResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.Equal(createdNarudzba.NarudzbaId, retrievedNarudzba.NarudzbaId);
        Assert.NotNull(retrievedNarudzba.StavkeNarudzbi);
        Assert.Single(retrievedNarudzba.StavkeNarudzbi);
    }

    [Fact]
    public async Task GetNonExistentResource_ShouldReturn404()
    {
        var response = await _client.GetAsync($"/api/konobar/{Guid.NewGuid()}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    private async Task<Konobar> CreateKonobar()
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

        var createdKonobar = JsonSerializer.Deserialize<Konobar>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return createdKonobar;
    }

    private async Task SeedTestData()
    {
        using var scope = _scopeFactory.CreateScope(); // ← koristi scope iz factory-a
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Resetiraj sve entitete
        context.StavkeNarudzbe.RemoveRange(context.StavkeNarudzbe);
        context.Narudzbe.RemoveRange(context.Narudzbe);
        context.Konobari.RemoveRange(context.Konobari);

        var konobar = new Konobar
        {
            IdKonobar = Guid.NewGuid(),
            Ime = "Test",
            Prezime = "Konobar",
            Telefon = "+385-111222333",
            Email = "test@test.com"
        };

        var narudzba = new Narudzba
        {
            NarudzbaId = Guid.NewGuid(),
            VrijemeNarudzbe = DateTime.Now.AddHours(-1),
            KonobarId = konobar.IdKonobar,
            Stol = "Stol01",
            Status = StatusNarudzbe.Zaprimljeno
        };

        var stavke = new List<StavkaNarudzbe>
        {
            new StavkaNarudzbe
            {
                StavkaNarudzbeId = Guid.NewGuid(),
                NarudzbaId = narudzba.NarudzbaId,
                Naziv = "Hamburger",
                Kolicina = 2,
                Cijena = 15.00m,
                AkcijskaPonuda = false,
                Status = StatusStavke.NaCekanju
            },
            new StavkaNarudzbe
            {
                StavkaNarudzbeId = Guid.NewGuid(),
                NarudzbaId = narudzba.NarudzbaId,
                Naziv = "Akcijski desert",
                Kolicina = 1,
                Cijena = 8.50m,
                AkcijskaPonuda = true,
                Status = StatusStavke.Pripremljeno
            }
        };

        context.Konobari.Add(konobar);
        context.Narudzbe.Add(narudzba);
        context.StavkeNarudzbe.AddRange(stavke);

        await context.SaveChangesAsync();
    }
}