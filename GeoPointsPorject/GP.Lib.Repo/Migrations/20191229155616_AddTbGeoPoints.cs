using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GP.Lib.Repo.Migrations
{
    public partial class AddTbGeoPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeoPoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OriginLat = table.Column<double>(nullable: false),
                    OriginLon = table.Column<double>(nullable: false),
                    DestinationLat = table.Column<double>(nullable: false),
                    DestinationLon = table.Column<double>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeoPoints_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "484094aa-befa-491d-9871-16cf9928951e");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b9c73cde-b224-4087-b929-1ca430d7551d", "AQAAAAEAACcQAAAAEGkWnjcavvvbRuhMiF392ToYigiA/lRrvVezKULarV4gfA/xQuTQpeHdb9TVQurTUg==", "2990c7bc-530e-40b5-a48d-ba3d29b282c7" });

            migrationBuilder.CreateIndex(
                name: "IX_GeoPoints_UserId",
                table: "GeoPoints",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeoPoints");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "02a978ff-5746-4f52-aa34-ff91548c7601");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "61930692-8cf5-417e-9395-efdc1c9d8dc9", "AQAAAAEAACcQAAAAEOsl+9Bj0IhKcbkfdf5+m8ffLgbiIDMkOBcssLc78nF0kQBeUQE7454d4zw0s6Kx6A==", "9ffdec1f-a7a4-45bf-88d1-39d50cb849db" });
        }
    }
}
