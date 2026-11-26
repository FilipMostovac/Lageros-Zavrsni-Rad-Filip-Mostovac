namespace Lageros.Models.NewFolder;

public class ZaposlenikRedakViewModel
{
    public int Id { get; set; }
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Radnomjesto { get; set; }
    public string Sektor { get; set; } = string.Empty;
    public bool Aktivan { get; set; }
    public int BrojZaduzenja { get; set; }

    public string PunoIme => $"{Ime} {Prezime}";
    public string Inicijali => $"{Ime[0]}{Prezime[0]}";
}