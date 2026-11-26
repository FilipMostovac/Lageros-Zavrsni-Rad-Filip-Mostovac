namespace Lageros.Models.ViewModels;

public class ZaposlenikDetailsViewModel
{
    public int Id { get; set; }
    public string PunoIme { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Radnomjesto { get; set; }
    public string Sektor { get; set; } = string.Empty;
    public bool Aktivan { get; set; }
    public int BrojZaduzenjaUkupno { get; set; }
    public int BrojAktivnihZaduzenja { get; set; }

    public List<ZaduzenjeRedakViewModel> Zaduzenja { get; set; } = new();
}

public class ZaduzenjeRedakViewModel
{
    public int Id { get; set; }
    public int OpremaId { get; set; }
    public string Oprema { get; set; } = string.Empty;
    public string InventarniBroj { get; set; } = string.Empty;
    public DateTime DatumZaduzenja { get; set; }
    public DateTime? DatumRazduzenja { get; set; }
    public string? Napomena { get; set; }
    public bool Aktivno => DatumRazduzenja == null;
}