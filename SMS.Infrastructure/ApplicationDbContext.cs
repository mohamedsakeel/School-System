using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public async Task<int> SaveEntityAsync()
        {
            return await base.SaveChangesAsync();
        }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<SubjectAssignment> SubjectAssignments { get; set; }
        public DbSet<ElectiveGroup> ElectiveGroups { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        public DbSet<ExamStructure> ExamStructures { get; set; }
        public DbSet<ExamSection> ExamSections { get; set; }
        public DbSet<ExamMarks> ExamMarks { get; set; }
        public DbSet<MCQAnswerSheet> MCQAnswerSheets { get; set; }
        public DbSet<ClassExamStructure> ClassExamStructures { get; set; }
        public DbSet<SubjectExamStructure> SubjectExamStructures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationship between Class and Student (One-to-Many)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassId);

            // Relationship between Class and Subject (One-to-Many)
            modelBuilder.Entity<Class>()
                .HasMany(c => c.Subjects)
                .WithMany(s => s.Classes)
                .UsingEntity(j => j.ToTable("ClassSubjects"));

            // Relationship between Class and ClassExamStructure (One-to-Many)
            modelBuilder.Entity<ClassExamStructure>()
                .HasOne(ce => ce.Class)
                .WithMany(c => c.ClassExamStructures)
                .HasForeignKey(ce => ce.ClassId);

            // Relationship between ExamStructure and ClassExamStructure (One-to-Many)
            modelBuilder.Entity<ClassExamStructure>()
                .HasOne(ce => ce.ExamStructure)
                .WithMany(es => es.ClassExamStructures)
                .HasForeignKey(ce => ce.ExamStructureId);

            // Relationship between Subject and SubjectExamStructure (One-to-Many)
            modelBuilder.Entity<SubjectExamStructure>()
                .HasOne(se => se.Subject)
                .WithMany(s => s.SubjectExamStructures)
                .HasForeignKey(se => se.SubjectId);

            // Relationship between ExamStructure and SubjectExamStructure (One-to-Many)
            modelBuilder.Entity<SubjectExamStructure>()
                .HasOne(se => se.ExamStructure)
                .WithMany(es => es.SubjectExamStructures)
                .HasForeignKey(se => se.ExamStructureId);
        }
    }
}
