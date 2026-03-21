using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniSchoolSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "100",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIX3em27td+b+Kpf4Xh6EdeK8U/cVim9Vv6PaK5lIWFeQzhsAtH12Gwp+aqIVu11TA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "101",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELvErj2GTOLxKN0n4kUzXpOhaF8s+0wD0n+60vP6yFh6tnzOKySpwlClHEMFOFsM2Q==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "102",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFLL6KvXz2hXgk5BO1mcHP/LaZJnZOCBEe/2KmAlRyEpNpo6ElZrnsB5R1NZqp2nFA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "103",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAED9S3PeoxmXvR3aFOWzFA9PJHOxaH4Q2O5ay/xdj5calf9iKBuW2bAfu+O64/Pb08Q==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "104",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGN2+SXGgb/jBdXaIryMpExW7tLw+FUFSpdCS+gPd/6PhAwtuRHwmBvbGsL1QDXcew==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "105",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENSX9/IGqCFMZJ7W129jSkdaLTeHV6bfPw8YlSCR8OP78t3u6teSh+JEOYdnGZUmjw==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "106",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEG8kcwNytYMn68j5VzbHl3k6yzlPXfbp5P3CTKD4+JvJ54qqfsfhG/UWSuEoIAyM1w==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "100",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEEiBlnC2o10DPLLcwfhSeU1PT8/l329z89a3053Cta/Npao2gegcazS8bHBW9N8Lvw==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "101",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMZsx7GSPeVrdB029TVlDC+Y16ld17nfREgsUbSxpEc/PfClSwF3d1cYlJuIZrXpxQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "102",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOvIkJ5fcM3TG/2BY96Sr470Upk9P12X+5f+NFWsys6zI1qfjHRzTQ18VvXHaqmM4w==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "103",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEEgprmmn8OOOmUCevefyW1M/AtxAdkltSxSvMe4ytk4nkQVaHT3zk6ebPt84zlEPiA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "104",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENaoyk+DlgQHuFPuL/BRmoOmvyagbkQcPdrOL/HcOd6dPhJJ5SdlBjqWcv8pFB/Nlg==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "105",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJ6UeBjfDsuNVpMPGFIXW+3slQqCHEJWloHrBUDeDerdiSokBp7qwLUHPArKlfTiLQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "106",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEBJbD1RSIFOOPPr6mhFvVS2/+LxoEDDH13kwYSx64y4FHetpG8kOc1avuNttsz/d3w==");
        }
    }
}
