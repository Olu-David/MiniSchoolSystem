using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace MiniSchoolSystem.Models
{
    public class AppDbContext:IdentityDbContext<UserDb>, IDataProtectionKeyContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserDb>(entity =>
            {
                entity.ToTable("AspNetUsers");
                entity.Property(m => m.FullName).HasMaxLength(200).IsRequired();
            });
            // Course to Module
            builder.Entity<CourseModule>()
                .HasOne(m => m.Course)
                .WithMany(c => c.CourseModules)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Teacher to Course
            builder.Entity<Course>()
                .HasOne(m => m.CourseTeacher)
                .WithMany(c => c.Courses)
                .HasForeignKey(p => p.TeacherID)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrollment to User (Identity)
            builder.Entity<LessonEnrollment>()
                .HasOne(m => m.UserDb)
                .WithMany(u => u.LessonEnrollments)
                .HasForeignKey(p => p.StudentId) // Matches the string property
                .OnDelete(DeleteBehavior.Restrict);

            // Enrollment to Lesson
            builder.Entity<LessonEnrollment>()
                .HasOne(m => m.Lesson)
                .WithMany(e => e.LessonEnrollments)
                .HasForeignKey(le => le.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            // Lesson to Module
            builder.Entity<Lesson>()
                .HasOne(m => m.CourseModule)
                .WithMany(c => c.Lessons)
                .HasForeignKey(m => m.CourseModuleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Lesson Content (The fix for the Global Filter warning)
            builder.Entity<LessonContent>()
                .HasOne(m => m.Lesson)
                .WithMany(m => m.LessonContents)
                .HasForeignKey(m => m.LessonId)
                .IsRequired(false) // Prevents crash when Lesson is filtered/deleted
                .OnDelete(DeleteBehavior.Restrict);

            // Teacher Section
            builder.Entity<TeacherSection>()
                .HasOne(m => m.Teacher)
                .WithMany(c => c.TeacherSections)
                .HasForeignKey(m => m.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Global Filter for Soft Delete
            builder.Entity<Lesson>().HasQueryFilter(l => !l.IsDeleted);
            builder.Entity<LessonEnrollment>().HasQueryFilter( le => !le.Lesson!.IsDeleted);


            //DatabaseLine important
            RolesSeedCreation(builder);
        }

        public void RolesSeedCreation(ModelBuilder builder)
        {
            //Role ID
            string SuperAdminRole = "1", AdminRole = "2", TeacherRole = "3", StudentRole = "4", ParentRole = "5";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = SuperAdminRole,
                    Name = "SuperAdmin",
                    ConcurrencyStamp = "C1",
                    NormalizedName = "SUPERADMIN"
                },

            new IdentityRole
            {
                Id = AdminRole,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "C2"

            },
             new IdentityRole
             {
                 Id = TeacherRole,
                 Name = "Teacher",
                 NormalizedName = "TEACHER",
                 ConcurrencyStamp = "C3"

             },
               new IdentityRole
               {                
                   Id = StudentRole,
                   Name = "Student",
                   ConcurrencyStamp = "C4",
                   NormalizedName = "STUDENT"
               },
                 new IdentityRole
                 {
                     Id = ParentRole,
                     Name = "Parent",
                     ConcurrencyStamp = "C5",
                     NormalizedName = "PARENT"
                 });



            // 2. USER IDs
            string superAdminUid = "100", AdminUid = "101",
                   teacherUid = "102", studentUid = "103", parentUid = "104", GodwinId = "105", EstherId = "106";

            var hasher = new PasswordHasher<UserDb>();

            // Super Admin
            var superAdmin = new UserDb
            {
                Id = superAdminUid,
                FullName = "Olusanya David Victor",
                UserName = "Olusanyadavid@yahoo.com",
                Email = "olusanyadavid@yahoo.com",
                NormalizedEmail = "OLUSANYADAVID@YAHOO.COM",
                NormalizedUserName = "OLUSANYADAVID@YAHOO.COM",
                EmailConfirmed = true,
                SecurityStamp = "STAMP100",
                ConcurrencyStamp = "CONC100",
                PhoneNumber = "0807212372"
            };
            superAdmin.PasswordHash = hasher.HashPassword(superAdmin, "Reciprocate1234.");
            
            // Admin
            var Admin = new UserDb
            {
                Id = AdminUid,
                FullName = " ADMIN",
                UserName = "Admin@school.com",
                Email = "Admin@school.com",
                NormalizedEmail = "ADMIN@SCHOOL.COM",
                NormalizedUserName = "ADMIN@SCHOOL.COM",
                EmailConfirmed = true,
                SecurityStamp = "STAMP101",
                ConcurrencyStamp = "CONC101",
                PhoneNumber = "0812329221"
            };
            Admin.PasswordHash = hasher.HashPassword(Admin, "PrimaryAdmin@1234.");


            // Teacher
            var teacher = new UserDb
            {
                Id = teacherUid,
                FullName = "Teacher",
                UserName = "Teacher@school.com",
                Email = "Teacher@school.com",
                NormalizedEmail = "TEACHER@SCHOOL.COM",
                NormalizedUserName = "TEACHER@SCHOOL.COM",
                EmailConfirmed = true,
                SecurityStamp = "STAMP102",
                ConcurrencyStamp = "CONC102",
                PhoneNumber = "09120292232"
            };
            teacher.PasswordHash = hasher.HashPassword(teacher, "Teacher@1234.");

            var Esther = new UserDb
            {
                Id = EstherId,
                FullName = "Esther OluwaSheyi",
                UserName = "Sheyi",
                Email = "emmanuelestheroluwasheyi17.com",
                NormalizedEmail = "EMMANUELESTHEROLUWASHEYI17.COM",
                NormalizedUserName = "ESTHER OLUWASHEYI",
                EmailConfirmed = true,
                SecurityStamp = "STAMP0107",
                ConcurrencyStamp = "CONC0107",
                PhoneNumber = "09120292232"
            };
            Esther.PasswordHash = hasher.HashPassword(Esther, "Esther21");



            var Godwin = new UserDb
            {
                Id = GodwinId,
                FullName = "Godwin Hyacinth",
                UserName = "HighLevel",
                Email = "godwinlevel139@gmail.com",
                NormalizedEmail = "GODWINLEVELL139@GMAIL.COM",
                NormalizedUserName = "GODWIN HYACINTH",
                EmailConfirmed = true,
                SecurityStamp = "STAMP0109",
                ConcurrencyStamp = "CONC0109",
                PhoneNumber = "09022341091"
            };
            Godwin.PasswordHash = hasher.HashPassword(Godwin, "G.oddylevel12");

            // Student
            var student = new UserDb
            {
                Id = studentUid,
                FullName = "Student User",
                UserName = "Student@school.com",
                Email = "Student@school.com",
                NormalizedEmail = "STUDENT@SCHOOL.COM",
                NormalizedUserName = "STUDENT@SCHOOL.COM",
                EmailConfirmed = true,
                SecurityStamp = "STAMP0108",
                ConcurrencyStamp = "CONC0180",
                PhoneNumber = "01290322332"
            };
            student.PasswordHash = hasher.HashPassword(student, "Student@1234.");

            // Parent
            var parent = new UserDb
            {
                Id = parentUid,
                FullName = "Parent User",
                UserName = "Parent@school.com",
                Email = "Parent@school.com",
                NormalizedEmail = "PARENT@SCHOOL.COM",
                NormalizedUserName = "PARENT@SCHOOL.COM",
                EmailConfirmed = true,
                SecurityStamp = "STAMP104",
                ConcurrencyStamp = "CONC104",
                PhoneNumber = "0810000000"
            };
            parent.PasswordHash = hasher.HashPassword(parent, "Parent@1234.");

            // Add all users
            builder.Entity<UserDb>().HasData(superAdmin, Admin, teacher, Esther, Godwin, student, parent);

            // 3. ASSIGN ROLES
            builder.Entity<IdentityUserRole<string>>().HasData(
           new IdentityUserRole<string> { RoleId = SuperAdminRole, UserId = superAdminUid },
         new IdentityUserRole<string> { RoleId = AdminRole, UserId = AdminUid },
         new IdentityUserRole<string> { RoleId = TeacherRole, UserId = teacherUid }, 
         new IdentityUserRole<string> { RoleId = StudentRole, UserId = studentUid }, 
          new IdentityUserRole<string> { RoleId = ParentRole, UserId = parentUid } ,
          new IdentityUserRole<string> { RoleId = TeacherRole, UserId = GodwinId },
           new IdentityUserRole<string> { RoleId = TeacherRole, UserId = EstherId }
           


 );         
        }


        

        public DbSet<Lesson> DbLesson {  get; set; }
        public DbSet<LessonContent> DbLessonContent { get; set;}
        public DbSet<LessonEnrollment>DbLessonEnrollments { get; set; }
        public DbSet<Teacher> DbTeacher {  get; set; }
        public DbSet<StudentModel> DbStudents { get; set; }
        public DbSet<Course> DbCourse {  get; set; }
        public DbSet<CourseModule>DbModules {  get; set; }
        public DbSet<StudentCourse> DbStudentCourses { get; set; }        
        public DbSet<Parent> DbParents { get; set; }
    }
}
