using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fabrizio.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "100000, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Audit_AddTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_EditTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Audit_AddTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_EditTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trips_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationBookings",
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

            migrationBuilder.CreateTable(
                name: "TravelBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Carrier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Departure = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Arrival = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Audit_AddTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_EditTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelBookings_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelBookings_Trips_TripId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TravelBookings_AccountId",
                table: "TravelBookings",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelBookings_TripId",
                table: "TravelBookings",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_AccountId",
                table: "Trips",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccomodationBookings");

            migrationBuilder.DropTable(
                name: "TravelBookings");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
