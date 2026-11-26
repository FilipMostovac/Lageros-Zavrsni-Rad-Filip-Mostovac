using Lageros.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lageros.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Sektor> Sektori { get; set; }
    public DbSet<Zaposlenik> Zaposlenici { get; set; }
    public DbSet<Oprema> Oprema { get; set; }
    public DbSet<Zaduzenje> Zaduzenja { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Inventarni broj mora biti jedinstven
        modelBuilder.Entity<Oprema>()
            .HasIndex(o => o.InventarniBroj)
            .IsUnique();

        // Ne dopuštamo brisanje sektora koji ima zaposlenike
        modelBuilder.Entity<Zaposlenik>()
            .HasOne(z => z.Sektor)
            .WithMany(s => s.Zaposlenici)
            .HasForeignKey(z => z.SektorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ne dopuštamo brisanje opreme/zaposlenika s poviješću
        modelBuilder.Entity<Zaduzenje>()
            .HasOne(z => z.Oprema)
            .WithMany(o => o.Zaduzenja)
            .HasForeignKey(z => z.OpremaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Zaduzenje>()
            .HasOne(z => z.Zaposlenik)
            .WithMany(z => z.Zaduzenja)
            .HasForeignKey(z => z.ZaposlenikId)
            .OnDelete(DeleteBehavior.Restrict);

        // Najviše jedno AKTIVNO zaduženje po opremi (filtrirani jedinstveni indeks)
        modelBuilder.Entity<Zaduzenje>()
            .HasIndex(z => z.OpremaId)
            .IsUnique()
            .HasFilter("[DatumRazduzenja] IS NULL")
            .HasDatabaseName("IX_Zaduzenja_AktivnoPoOpremi");

        // Jedinstveni email za zaposlenika
        modelBuilder.Entity<Zaposlenik>()
            .HasIndex(z => z.Email)
            .IsUnique();
    }
}