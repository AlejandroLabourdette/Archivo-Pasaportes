using Microsoft.EntityFrameworkCore.Migrations;

namespace ArchivoDePasaportes.Data.Migrations
{
    public partial class AddPermanentTravelsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PermanentTravels",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<long>(nullable: false),
                    PassportId = table.Column<long>(nullable: false),
                    OccupationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermanentTravels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermanentTravels_Occupations_OccupationId",
                        column: x => x.OccupationId,
                        principalTable: "Occupations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PermanentTravels_Passports_PassportId",
                        column: x => x.PassportId,
                        principalTable: "Passports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermanentTravels_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermanentTravels_OccupationId",
                table: "PermanentTravels",
                column: "OccupationId");

            migrationBuilder.CreateIndex(
                name: "IX_PermanentTravels_PassportId",
                table: "PermanentTravels",
                column: "PassportId");

            migrationBuilder.CreateIndex(
                name: "IX_PermanentTravels_TicketId",
                table: "PermanentTravels",
                column: "TicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermanentTravels");
        }
    }
}
