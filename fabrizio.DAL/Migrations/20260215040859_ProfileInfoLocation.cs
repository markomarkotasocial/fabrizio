using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fabrizio.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ProfileInfoLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    City = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Audit_AddTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_EditTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountInfos",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    PreferredLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PreferredCurrency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDarkMode = table.Column<bool>(type: "bit", nullable: false),
                    HomeLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Audit_AddTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_EditTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInfos", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_AccountInfos_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountInfos_Locations_HomeLocationId",
                        column: x => x.HomeLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountInfos_HomeLocationId",
                table: "AccountInfos",
                column: "HomeLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CountryCode_City",
                table: "Locations",
                columns: new[] { "CountryCode", "City" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountInfos");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
