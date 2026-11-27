namespace Lageros.Models.ViewModels;

public record DonutSegment(string Boja, int Broj);

public class DonutViewModel
{
    public int Velicina { get; set; } = 84;
    public string? Oznaka { get; set; }
    public List<DonutSegment> Segmenti { get; set; } = new();
    public int Ukupno => Segmenti.Sum(s => s.Broj);
}

public class VrstaStatistikaViewModel
{
    public string Naslov { get; set; } = string.Empty;
    public int UUpotrebi { get; set; }
    public int NaSkladistu { get; set; }
    public int NaServisu { get; set; }

    public DonutViewModel Donut => new()
    {
        Segmenti = new List<DonutSegment>
        {
            new("var(--c-zaduzeno)", UUpotrebi),
            new("var(--c-skladiste)", NaSkladistu),
            new("var(--c-servis)", NaServisu)
        }
    };
}

public class MjesecStatistikaViewModel
{
    public string Oznaka { get; set; } = string.Empty;
    public int Zaduzeno { get; set; }
    public int Vraceno { get; set; }
}

public class DashboardViewModel
{
    public string Datum { get; set; } = string.Empty;
    public string Pozdrav { get; set; } = string.Empty;

    public int Ukupno { get; set; }
    public int UUpotrebi { get; set; }
    public int NaSkladistu { get; set; }
    public int NaServisu { get; set; }
    public int Otpisano { get; set; }
    public int ZaposlenikaSOpremom { get; set; }

    public List<VrstaStatistikaViewModel> PoVrstama { get; set; } = new();
    public List<MjesecStatistikaViewModel> PoMjesecima { get; set; } = new();
    public int VrhOsi { get; set; } = 4;

    public DonutViewModel UkupnoDonut => new()
    {
        Velicina = 130,
        Oznaka = "ukupno",
        Segmenti = new List<DonutSegment>
        {
            new("var(--c-zaduzeno)", UUpotrebi),
            new("var(--c-skladiste)", NaSkladistu),
            new("var(--c-servis)", NaServisu)
        }
    };
}