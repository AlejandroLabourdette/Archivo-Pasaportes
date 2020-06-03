using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ArchivoDePasaportes.Data.Migrations
{
    public partial class AddDroppedPassportsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DroppedPassports",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    OwnerId = table.Column<string>(nullable: false),
                    PassportTypeId = table.Column<byte>(nullable: false),
                    SourceId = table.Column<int>(nullable: true),
                    ExpeditionDate = table.Column<DateTime>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    DropCauseId = table.Column<byte>(nullable: false),
                    Details = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DroppedPassports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DroppedPassports_DropCauses_DropCauseId",
                        column: x => x.DropCauseId,
                        principalTable: "DropCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DroppedPassports_People_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DroppedPassports_PassportTypes_PassportTypeId",
                        column: x => x.PassportTypeId,
                        principalTable: "PassportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DroppedPassports_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DroppedPassports_DropCauseId",
                table: "DroppedPassports",
                column: "DropCauseId");

            migrationBuilder.CreateIndex(
                name: "IX_DroppedPassports_OwnerId",
                table: "DroppedPassports",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_DroppedPassports_PassportTypeId",
                table: "DroppedPassports",
                column: "PassportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DroppedPassports_SourceId",
                table: "DroppedPassports",
                column: "SourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DroppedPassports");
        }
    }
}
