using Microsoft.EntityFrameworkCore.Migrations;

namespace ArchivoDePasaportes.Data.Migrations
{
    public partial class PopulateSourcesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Sources (Name, Description) VALUES ('Universidad de la Habana', 'Ubicada en el corazón del Vedado')");
            migrationBuilder.Sql("INSERT INTO Sources (Name, Description) VALUES ('Ministerio de Relaciones Exteriores', 'Tambien conocido por sus siglas MINREX')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
