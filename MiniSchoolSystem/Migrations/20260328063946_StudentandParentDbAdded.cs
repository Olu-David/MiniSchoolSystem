using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiniSchoolSystem.Migrations
{
    /// <inheritdoc />
    public partial class StudentandParentDbAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchiveAt",
                table: "DbModules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DbLessonEnrollments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "DbLessonContent",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DbLessonContent",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "DbLessonContent",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DbParents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbParents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbParents_AspNetUsers_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "100",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEESPxTP0d4/kK6IFE1tdIuoQ7v8pypkhuycfHCQCBfepGmJ+p+hQXu74Gh73S6TEOQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "101",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMWZEwmHmWYT6MD+0dLKrhiZR/I3SnJ/sKi49HcpA5KGbs3rz8KVIX9fX358EzTCFQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "102",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGwBwY6TTmi7932xvWzyFl0jgefFu26ljLdWtOUhmCGJflZNKTu1Rj5uCU9wZBLnpg==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "103",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEA8OOXNWySaP2ZYuMAotI7QLZmkGw8ytWGR1EJ2Ng3Oazd5qdXId1CvzoYm50/Bqrw==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "104",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEM+vvOuKLAXjSws3Neh9+g1Ab/I5v7mXPg1DjiEu0bXxH+8uqiaVlf8hgPNC7fVsSw==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "105",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGDnj5btgVZbC2Qrhg1zUP4Ti/NlMRMTsd5MM/kfOWLdmE65GzVj09bjNqAuclHBfA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "106",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELjMhjic/USocU2LK4pizN82ci1sMcjPhxpeIjGVLyLWJLfS0qwtECmIGc1mB3RpxA==");

            migrationBuilder.CreateIndex(
                name: "IX_DbParents_ParentId",
                table: "DbParents",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbParents");

            migrationBuilder.DropColumn(
                name: "ArchiveAt",
                table: "DbModules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DbLessonEnrollments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DbLessonContent");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DbLessonContent");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "DbLessonContent");

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
    }
}
