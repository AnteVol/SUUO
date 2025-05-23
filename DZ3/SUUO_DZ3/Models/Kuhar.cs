namespace SUUO_DZ3.Models;

public class Kuhar
{
    public Guid IdKuhar { get; set; }
    
    public string Ime { get; set; }
    
    public string Prezime { get; set; }
    
    public string Telefon { get; set; }

    public string Email { get; set; }
        
    public bool Aktivan { get; set; } = true;
    
    public ICollection<string> Specijaliteti { get; set; } = new List<string>();
    
    public virtual ICollection<StavkaNarudzbe> StavkeNarudzbi { get; set; } = new List<StavkaNarudzbe>();
    
}