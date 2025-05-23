namespace SUUO_DZ3.Models;

public class Konobar
{
    public Guid IdKonobar { get; set; } = Guid.NewGuid();
    
    public string Ime { get; set; }
    
    public string Prezime { get; set; }
    
    public string Telefon { get; set; }
    
    public string Email { get; set; }
        
    public bool Aktivan { get; set; } = true;

}