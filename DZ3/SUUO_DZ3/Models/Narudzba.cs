using SUUO_DZ3.Models.Enums;

namespace SUUO_DZ3.Models;

public class Narudzba
{
    public Guid NarudzbaId { get; set; } = Guid.NewGuid();
    
    public DateTime VrijemeNarudzbe { get; set; }
    
    public Guid KonobarId { get; set; }
    
    public Konobar Konobar { get; set; }
    
    public string Stol { get; set; }
    
    public StatusNarudzbe Status { get; set; }
    
    public MetodaPlacanja? MetodaPlacanja { get; set; }
    
    public ICollection<StavkaNarudzbe> StavkeNarudzbi { get; set; }
    
    
    public decimal UkupnaCijena => StavkeNarudzbi?.Sum(s => s.UkupnaCijena) ?? 0;
    
    public int UkupnoAkcijskihPonuda => StavkeNarudzbi?.Sum(s => s.UkupnoAkcijskihPonuda) ?? 0;
        
    public int BrojStavki => StavkeNarudzbi?.Count ?? 0;
}
