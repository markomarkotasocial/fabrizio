using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fabrizio.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AccommodationBookingFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccomodationBookings");

            migrationBuilder.CreateTable(
                name: "AccommodationBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    From = table.Column<DateTime>(type: "datetime2", nullable: true),
                    To = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Audit_AddTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_EditTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccommodationBookings_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccommodationBookings_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationBookings_AccountId",
                table: "AccommodationBookings",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationBookings_TripId",
                table: "AccommodationBookings",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccommodationBookings");

            migrationBuilder.CreateTable(
                name: "AccomodationBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    From = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    To = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Audit_AddTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_EditTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationBookings_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccomodationBookings_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccomodationBookings_AccountId",
                table: "AccomodationBookings",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccomodationBookings_TripId",
                table: "AccomodationBookings",
                column: "TripId");
        }
    }
}
