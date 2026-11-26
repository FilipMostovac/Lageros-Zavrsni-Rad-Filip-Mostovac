using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Lageros.Models;

public static class EnumExtensions
{
    public static string Naziv(this Enum vrijednost)
    {
        var member = vrijednost.GetType().GetMember(vrijednost.ToString()).FirstOrDefault();
        var display = member?.GetCustomAttribute<DisplayAttribute>();
        return display?.GetName() ?? vrijednost.ToString();
    }
}