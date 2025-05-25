using System.Text.Json.Serialization;
using SUUO_DZ3.Models.Enums;

namespace SUUO_DZ3.Models;

public class StavkaNarudzbe
{
    public Guid StavkaNarudzbeId { get; set; }
    
    public Guid NarudzbaId { get; set; }
    
    public Narudzba? Narudzba { get; set; }
    
    public Guid? KuharId { get; set; }
    
    public Kuhar? Kuhar { get; set; }
    
    public string Naziv { get; set; }
    public int Kolicina { get; set; } = 1;
    
    public decimal Cijena { get; set; }
    
    public bool AkcijskaPonuda { get; set; }

    public StatusStavke Status { get; set; } = StatusStavke.NaCekanju;
        
    public decimal UkupnaCijena => Kolicina * Cijena;
        
    public int UkupnoAkcijskihPonuda => (AkcijskaPonuda ? 1 : 0) * Kolicina;
}