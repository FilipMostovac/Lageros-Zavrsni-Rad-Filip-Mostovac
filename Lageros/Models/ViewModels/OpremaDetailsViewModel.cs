namespace Lageros.Models.ViewModels;

public class OpremaDetailsViewModel
{
    public int Id { get; set; }
    public string InventarniBroj { get; set; } = string.Empty;
    public VrstaOpreme Vrsta { get; set; }
    public string? Podvrsta { get; set; }
    public string Proizvodac { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? SerijskiBroj { get; set; }
    public StatusOpreme Status { get; set; }
    public DateTime? DatumNabave { get; set; }
    public string? Napomena { get; set; }

    public List<OpremaPovijestRedakViewModel> Povijest { get; set; } = new();
}

public class OpremaPovijestRedakViewModel
{
    public int ZaposlenikId { get; set; }
    public string Zaposlenik { get; set; } = string.Empty;
    public DateTime DatumZaduzenja { get; set; }
    public DateTime? DatumRazduzenja { get; set; }
    public string? Napomena { get; set; }
    public string? Administrator { get; set; }
    public bool Aktivno => DatumRazduzenja == null;
}