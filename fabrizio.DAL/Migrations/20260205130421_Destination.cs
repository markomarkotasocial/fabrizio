using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fabrizio.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Destination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destination",
                table: "Trips");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Trips",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Audit_AddTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_EditTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Destinations_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Destinations_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Destinations_AccountId",
                table: "Destinations",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Destinations_TripId",
                table: "Destinations",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Destinations");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Trips");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "Trips",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
