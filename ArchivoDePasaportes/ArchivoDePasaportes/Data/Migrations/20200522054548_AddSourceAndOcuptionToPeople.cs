using Microsoft.EntityFrameworkCore.Migrations;

namespace ArchivoDePasaportes.Data.Migrations
{
    public partial class AddSourceAndOcuptionToPeople : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ocupation",
                table: "People",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "People",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_People_SourceId",
                table: "People",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_People_Sources_SourceId",
                table: "People",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Sources_SourceId",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_People_SourceId",
                table: "People");

            migrationBuilder.DropColumn(
                name: "Ocupation",
                table: "People");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "People");
        }
    }
}
