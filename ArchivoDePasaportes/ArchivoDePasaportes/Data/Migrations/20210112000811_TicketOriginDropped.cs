using Microsoft.EntityFrameworkCore.Migrations;

namespace ArchivoDePasaportes.Data.Migrations
{
    public partial class TicketOriginDropped : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passports_Sources_SourceId",
                table: "Passports");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Countries_OriginCountryId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_OriginCountryId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "OriginCountryId",
                table: "Tickets");

            migrationBuilder.AlterColumn<int>(
                name: "SourceId",
                table: "Passports",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Passports_Sources_SourceId",
                table: "Passports",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passports_Sources_SourceId",
                table: "Passports");

            migrationBuilder.AddColumn<byte>(
                name: "OriginCountryId",
                table: "Tickets",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<int>(
                name: "SourceId",
                table: "Passports",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_OriginCountryId",
                table: "Tickets",
                column: "OriginCountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Passports_Sources_SourceId",
                table: "Passports",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Countries_OriginCountryId",
                table: "Tickets",
                column: "OriginCountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
