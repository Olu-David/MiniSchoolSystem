using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniSchoolSystem.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "100",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGOPfn55t/bR2n/va428RT4dYjAQmfjug93obXjHbzEbrKniYxJ4UnpGjV3GwocmLg==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "101",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGyT0BDjsEW/kvmP0894wpPHu1c+tgdTzUqFpBPHSO+8zYugSgivjICgvVu4bBSCuQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "102",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEE0HZsbAc30OctBFKCZu82Gj5clwi8Oy/1Gr8Lxm3Jkbu1A8UZ1BqvxSq9sPeJt6Qg==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "103",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMplkdIiHrD6h1KxMGrWInsbEF9NEFP2OKD1tQQAwpoK6EbJP0y4h26u9dk1xxHL5g==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "104",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOIGjmzl29LuMguYMgpKI15BcWV2ryPoelD+5VzlkO++OaD1uuRXIj0ZnTovuUmRMA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "100",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGa64htVQzybg2u/avK4d3XjHmoKtwaMwadBWy2qt5ikr2UXO7P9qPF5HeSFPleohA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "101",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIy8NOSanRCKHABKFTSTeP1JRKNX7yGr3wrypZdMEZ0tplubK/k0eicqS/TAKYBAxQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "102",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJwB0j59B2FQYUSBQMUdzycvChhz9aPFtHq0lPjzMG67pGhjnofLYsP4OUjfTq8+lg==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "103",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEA/51iEnEdERXGrx8hc2YcJMSKTSIYg+vU0yWsc03+YzEU+n27nN0BPWI8bEKWLxbA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "104",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEO8Eoh7DlIuT6BlIVztJvJ+m08YL36GTXK069JGLfqRJk3TPLm1zq6RjDR3DQZLoFQ==");
        }
    }
}
