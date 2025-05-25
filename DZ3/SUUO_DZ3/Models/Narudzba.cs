using System.Text.Json.Serialization;
using SUUO_DZ3.Models.Enums;

namespace SUUO_DZ3.Models;

public class Narudzba
{
    public Guid NarudzbaId { get; set; } = Guid.NewGuid();
    
    public DateTime VrijemeNarudzbe { get; set; }
    
    public Guid KonobarId { get; set; }
    
    public Konobar? Konobar { get; set; }
    
    [JsonPropertyName("stol")]
    public string Stol { get; set; }
    
    public StatusNarudzbe Status { get; set; } = StatusNarudzbe.Zaprimljeno;
    
    public MetodaPlacanja? MetodaPlacanja { get; set; }
    
    [JsonPropertyName("stavkeNarudzbi")]
    public List<StavkaNarudzbe> StavkeNarudzbi { get; set; } = new List<StavkaNarudzbe>();
    
    [JsonPropertyName("ukupnaCijena")]
    public decimal UkupnaCijena => StavkeNarudzbi?.Sum(s => s.UkupnaCijena) ?? 0;
    
    public int UkupnoAkcijskihPonuda => StavkeNarudzbi?.Sum(s => s.UkupnoAkcijskihPonuda) ?? 0;
        
    public int BrojStavki => StavkeNarudzbi?.Count ?? 0;
}