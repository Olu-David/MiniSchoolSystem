using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSchoolSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    Xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: true),
                    LessonId = table.Column<int>(type: "integer", nullable: true),
                    UserSection = table.Column<int>(type: "integer", nullable: true),
                    CourseModuleId = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DbStudents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentName = table.Column<string>(type: "text", nullable: true),
                    StudentEmail = table.Column<string>(type: "text", nullable: true),
                    StudentSection = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbStudents_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DbTeacher",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    TeacherId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbTeacher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbTeacher_AspNetUsers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DbCourse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CourseUserbID = table.Column<string>(type: "text", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TeacherID = table.Column<int>(type: "integer", nullable: false),
                    CourseSections = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbCourse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbCourse_AspNetUsers_CourseUserbID",
                        column: x => x.CourseUserbID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DbCourse_DbTeacher_TeacherID",
                        column: x => x.TeacherID,
                        principalTable: "DbTeacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TSection = table.Column<int>(type: "integer", nullable: true),
                    TeacherId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherSection_DbTeacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "DbTeacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DbModules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CourseSections = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbModules_DbCourse_CourseId",
                        column: x => x.CourseId,
                        principalTable: "DbCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbModules_DbTeacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "DbTeacher",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DbStudentCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    UserDbID = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbStudentCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbStudentCourses_AspNetUsers_UserDbID",
                        column: x => x.UserDbID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DbStudentCourses_DbCourse_CourseId",
                        column: x => x.CourseId,
                        principalTable: "DbCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbStudentCourses_DbStudents_StudentId",
                        column: x => x.StudentId,
                        principalTable: "DbStudents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DbLesson",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    LessonSection = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CourseModuleId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    StudentModelId = table.Column<int>(type: "integer", nullable: true),
                    UserDbId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbLesson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbLesson_AspNetUsers_UserDbId",
                        column: x => x.UserDbId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DbLesson_DbModules_CourseModuleId",
                        column: x => x.CourseModuleId,
                        principalTable: "DbModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbLesson_DbStudents_StudentModelId",
                        column: x => x.StudentModelId,
                        principalTable: "DbStudents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DbLessonContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: true),
                    VideoUrl = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: true),
                    LessonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbLessonContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbLessonContent_DbLesson_LessonId",
                        column: x => x.LessonId,
                        principalTable: "DbLesson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbLessonContent_DbStudents_StudentId",
                        column: x => x.StudentId,
                        principalTable: "DbStudents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DbLessonEnrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LessonId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    LessonProgress = table.Column<decimal>(type: "numeric", nullable: false),
                    EnrolledTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Sections = table.Column<int>(type: "integer", nullable: false),
                    StudentName = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbLessonEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbLessonEnrollments_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbLessonEnrollments_DbLesson_LessonId",
                        column: x => x.LessonId,
                        principalTable: "DbLesson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbLessonEnrollments_DbStudents_StudentName",
                        column: x => x.StudentName,
                        principalTable: "DbStudents",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", "C1", "SuperAdmin", "SUPERADMIN" },
                    { "2", "C2", "Admin", "ADMIN" },
                    { "3", "C3", "Teacher", "TEACHER" },
                    { "4", "C4", "Student", "STUDENT" },
                    { "5", "C5", "Parent", "PARENT" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CourseModuleId", "Email", "EmailConfirmed", "FullName", "LessonId", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "StudentId", "TwoFactorEnabled", "UserName", "UserSection" },
                values: new object[,]
                {
                    { "100", 0, "CONC100", null, "olusanyadavid@yahoo.com", true, "Olusanya David Victor", null, false, null, "OLUSANYADAVID@YAHOO.COM", "OLUSANYADAVID@YAHOO.COM", "AQAAAAIAAYagAAAAEEiBlnC2o10DPLLcwfhSeU1PT8/l329z89a3053Cta/Npao2gegcazS8bHBW9N8Lvw==", "0807212372", false, "STAMP100", null, false, "Olusanyadavid@yahoo.com", null },
                    { "101", 0, "CONC101", null, "Admin@school.com", true, " ADMIN", null, false, null, "ADMIN@SCHOOL.COM", "ADMIN@SCHOOL.COM", "AQAAAAIAAYagAAAAEMZsx7GSPeVrdB029TVlDC+Y16ld17nfREgsUbSxpEc/PfClSwF3d1cYlJuIZrXpxQ==", "0812329221", false, "STAMP101", null, false, "Admin@school.com", null },
                    { "102", 0, "CONC102", null, "Teacher@school.com", true, "Teacher", null, false, null, "TEACHER@SCHOOL.COM", "TEACHER@SCHOOL.COM", "AQAAAAIAAYagAAAAEOvIkJ5fcM3TG/2BY96Sr470Upk9P12X+5f+NFWsys6zI1qfjHRzTQ18VvXHaqmM4w==", "09120292232", false, "STAMP102", null, false, "Teacher@school.com", null },
                    { "103", 0, "CONC0180", null, "Student@school.com", true, "Student User", null, false, null, "STUDENT@SCHOOL.COM", "STUDENT@SCHOOL.COM", "AQAAAAIAAYagAAAAEEgprmmn8OOOmUCevefyW1M/AtxAdkltSxSvMe4ytk4nkQVaHT3zk6ebPt84zlEPiA==", "01290322332", false, "STAMP0108", null, false, "Student@school.com", null },
                    { "104", 0, "CONC104", null, "Parent@school.com", true, "Parent User", null, false, null, "PARENT@SCHOOL.COM", "PARENT@SCHOOL.COM", "AQAAAAIAAYagAAAAENaoyk+DlgQHuFPuL/BRmoOmvyagbkQcPdrOL/HcOd6dPhJJ5SdlBjqWcv8pFB/Nlg==", "0810000000", false, "STAMP104", null, false, "Parent@school.com", null },
                    { "105", 0, "CONC0109", null, "godwinlevel139@gmail.com", true, "Godwin Hyacinth", null, false, null, "GODWINLEVELL139@GMAIL.COM", "GODWIN HYACINTH", "AQAAAAIAAYagAAAAEJ6UeBjfDsuNVpMPGFIXW+3slQqCHEJWloHrBUDeDerdiSokBp7qwLUHPArKlfTiLQ==", "09022341091", false, "STAMP0109", null, false, "HighLevel", null },
                    { "106", 0, "CONC0107", null, "emmanuelestheroluwasheyi17.com", true, "Esther OluwaSheyi", null, false, null, "EMMANUELESTHEROLUWASHEYI17.COM", "ESTHER OLUWASHEYI", "AQAAAAIAAYagAAAAEBJbD1RSIFOOPPr6mhFvVS2/+LxoEDDH13kwYSx64y4FHetpG8kOc1avuNttsz/d3w==", "09120292232", false, "STAMP0107", null, false, "Sheyi", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1", "100" },
                    { "2", "101" },
                    { "3", "102" },
                    { "4", "103" },
                    { "5", "104" },
                    { "3", "105" },
                    { "3", "106" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CourseModuleId",
                table: "AspNetUsers",
                column: "CourseModuleId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DbCourse_CourseUserbID",
                table: "DbCourse",
                column: "CourseUserbID");

            migrationBuilder.CreateIndex(
                name: "IX_DbCourse_TeacherID",
                table: "DbCourse",
                column: "TeacherID");

            migrationBuilder.CreateIndex(
                name: "IX_DbLesson_CourseModuleId",
                table: "DbLesson",
                column: "CourseModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_DbLesson_StudentModelId",
                table: "DbLesson",
                column: "StudentModelId");

            migrationBuilder.CreateIndex(
                name: "IX_DbLesson_UserDbId",
                table: "DbLesson",
                column: "UserDbId");

            migrationBuilder.CreateIndex(
                name: "IX_DbLessonContent_LessonId",
                table: "DbLessonContent",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_DbLessonContent_StudentId",
                table: "DbLessonContent",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_DbLessonEnrollments_LessonId",
                table: "DbLessonEnrollments",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_DbLessonEnrollments_StudentId",
                table: "DbLessonEnrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_DbLessonEnrollments_StudentName",
                table: "DbLessonEnrollments",
                column: "StudentName");

            migrationBuilder.CreateIndex(
                name: "IX_DbModules_CourseId",
                table: "DbModules",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_DbModules_TeacherId",
                table: "DbModules",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_DbStudentCourses_CourseId",
                table: "DbStudentCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_DbStudentCourses_StudentId",
                table: "DbStudentCourses",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_DbStudentCourses_UserDbID",
                table: "DbStudentCourses",
                column: "UserDbID");

            migrationBuilder.CreateIndex(
                name: "IX_DbStudents_StudentId",
                table: "DbStudents",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DbTeacher_TeacherId",
                table: "DbTeacher",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSection_TeacherId",
                table: "TeacherSection",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_DbModules_CourseModuleId",
                table: "AspNetUsers",
                column: "CourseModuleId",
                principalTable: "DbModules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DbCourse_AspNetUsers_CourseUserbID",
                table: "DbCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_DbTeacher_AspNetUsers_TeacherId",
                table: "DbTeacher");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "DbLessonContent");

            migrationBuilder.DropTable(
                name: "DbLessonEnrollments");

            migrationBuilder.DropTable(
                name: "DbStudentCourses");

            migrationBuilder.DropTable(
                name: "TeacherSection");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "DbLesson");

            migrationBuilder.DropTable(
                name: "DbStudents");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "DbModules");

            migrationBuilder.DropTable(
                name: "DbCourse");

            migrationBuilder.DropTable(
                name: "DbTeacher");
        }
    }
}
