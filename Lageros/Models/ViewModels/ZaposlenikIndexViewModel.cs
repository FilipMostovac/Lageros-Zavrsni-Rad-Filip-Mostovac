using System.ComponentModel.DataAnnotations;
using Lageros.Models.NewFolder;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lageros.Models.ViewModels;

public class ZaposlenikIndexViewModel
{
    [Display(Name = "Pretraga")]
    public string? Trazi { get; set; }

    [Display(Name = "Sektor")]
    public int? SektorId { get; set; }

    [Display(Name = "Prikaži neaktivne")]
    public bool PrikaziNeaktivne { get; set; }

    public SelectList? Sektori { get; set; }
    public List<ZaposlenikRedakViewModel> Zaposlenici { get; set; } = new();
}