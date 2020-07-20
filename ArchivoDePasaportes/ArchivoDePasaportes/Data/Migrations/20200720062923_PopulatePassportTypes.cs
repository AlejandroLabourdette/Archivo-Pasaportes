using Microsoft.EntityFrameworkCore.Migrations;

namespace ArchivoDePasaportes.Data.Migrations
{
    public partial class PopulatePassportTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO PassportTypes (Id, Name) VALUES (1, 'Diplomático')");
            migrationBuilder.Sql("INSERT INTO PassportTypes (Id, Name) VALUES (2, 'Oficial')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
