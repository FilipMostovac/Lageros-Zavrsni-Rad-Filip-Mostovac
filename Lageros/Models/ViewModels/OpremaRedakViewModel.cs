namespace Lageros.Models.ViewModels;

public class OpremaRedakViewModel
{
    public int Id { get; set; }
    public string InventarniBroj { get; set; } = string.Empty;
    public VrstaOpreme Vrsta { get; set; }
    public string Proizvodac { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? Podvrsta { get; set; }
    public string? SerijskiBroj { get; set; }
    public StatusOpreme Status { get; set; }
    public string? Zaposlenik { get; set; }
    public DateTime? ZaduzenoOd { get; set; }
}