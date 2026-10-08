using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WikimediaFacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PageId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Extract = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    FullUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    EventDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RelatedPeople = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    IsUserFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikimediaFacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikimediaFacts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WikimediaMonuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PageId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Extract = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    FullUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ConstructionYear = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Architect = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsUserFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikimediaMonuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikimediaMonuments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WikimediaPeople",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PageId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Extract = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    FullUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeathDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BirthPlace = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Nationality = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Occupation = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IsUserFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikimediaPeople", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikimediaPeople_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WikimediaPlaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PageId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Extract = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    FullUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsUserFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikimediaPlaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WikimediaPlaces_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaFacts_Category",
                table: "WikimediaFacts",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaFacts_EventDate",
                table: "WikimediaFacts",
                column: "EventDate");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaFacts_PageId",
                table: "WikimediaFacts",
                column: "PageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaFacts_UserId",
                table: "WikimediaFacts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaMonuments_City",
                table: "WikimediaMonuments",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaMonuments_Country",
                table: "WikimediaMonuments",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaMonuments_PageId",
                table: "WikimediaMonuments",
                column: "PageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaMonuments_Type",
                table: "WikimediaMonuments",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaMonuments_UserId",
                table: "WikimediaMonuments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaPeople_Nationality",
                table: "WikimediaPeople",
                column: "Nationality");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaPeople_Occupation",
                table: "WikimediaPeople",
                column: "Occupation");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaPeople_PageId",
                table: "WikimediaPeople",
                column: "PageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaPeople_UserId",
                table: "WikimediaPeople",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaPlaces_City",
                table: "WikimediaPlaces",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaPlaces_Country",
                table: "WikimediaPlaces",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaPlaces_PageId",
                table: "WikimediaPlaces",
                column: "PageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaPlaces_Type",
                table: "WikimediaPlaces",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_WikimediaPlaces_UserId",
                table: "WikimediaPlaces",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WikimediaFacts");

            migrationBuilder.DropTable(
                name: "WikimediaMonuments");

            migrationBuilder.DropTable(
                name: "WikimediaPeople");

            migrationBuilder.DropTable(
                name: "WikimediaPlaces");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
