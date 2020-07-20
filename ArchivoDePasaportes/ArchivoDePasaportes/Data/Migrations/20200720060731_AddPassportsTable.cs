using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ArchivoDePasaportes.Data.Migrations
{
    public partial class AddPassportsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Passports",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PassportNo = table.Column<string>(nullable: false),
                    OwnerId = table.Column<long>(nullable: false),
                    PassportTypeId = table.Column<byte>(nullable: false),
                    SourceId = table.Column<int>(nullable: true),
                    ExpeditionDate = table.Column<DateTime>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    IsPassportArchived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passports_People_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Passports_PassportTypes_PassportTypeId",
                        column: x => x.PassportTypeId,
                        principalTable: "PassportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Passports_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Passports_OwnerId",
                table: "Passports",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Passports_PassportTypeId",
                table: "Passports",
                column: "PassportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Passports_SourceId",
                table: "Passports",
                column: "SourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Passports");
        }
    }
}
