using System.ComponentModel.DataAnnotations;

namespace Lageros.Models;

public enum StatusOpreme
{
    [Display(Name = "Na skladištu")]
    NaSkladistu = 1,

    [Display(Name = "Zaduženo")]
    Zaduzeno = 2,

    [Display(Name = "Na servisu")]
    NaServisu = 3,

    [Display(Name = "Otpisano")]
    Otpisano = 4
}