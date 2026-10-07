using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWikimediaEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "WikimediaFacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Extract = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FullUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RelatedPeople = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsUserFavorite = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Extract = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FullUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConstructionYear = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Architect = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsUserFavorite = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Extract = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FullUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeathDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BirthPlace = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsUserFavorite = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Extract = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FullUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsUserFavorite = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
