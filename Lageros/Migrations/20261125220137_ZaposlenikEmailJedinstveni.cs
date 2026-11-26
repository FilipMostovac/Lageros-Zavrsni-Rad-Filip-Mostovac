using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lageros.Migrations
{
    /// <inheritdoc />
    public partial class ZaposlenikEmailJedinstveni : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Zaposlenici_Email",
                table: "Zaposlenici",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Zaposlenici_Email",
                table: "Zaposlenici");
        }
    }
}
