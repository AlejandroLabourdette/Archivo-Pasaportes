using Microsoft.EntityFrameworkCore.Migrations;

namespace ArchivoDePasaportes.Data.Migrations
{
    public partial class PopulateDropCauseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO DropCauses (Id, Name) VALUES (1, 'Expiración')");
            migrationBuilder.Sql("INSERT INTO DropCauses (Id, Name) VALUES (2, 'Deterioro')");
            migrationBuilder.Sql("INSERT INTO DropCauses (Id, Name) VALUES (3, 'Pérdida')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
