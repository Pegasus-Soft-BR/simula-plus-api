using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using Domain;
using Domain.DTOs;
using MockExams.Infra.Database.Mapping;

namespace MockExams.Infra.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public ApplicationDbContext() { }

    public DbSet<Exam> Exams { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<ExamAttempt> ExamAttempts { get; set; }
    public DbSet<Answer> Answers { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new AnswerMap(modelBuilder.Entity<Answer>());

        base.OnModelCreating(modelBuilder);

        this.SetUtcOnDatabase(modelBuilder);
    }
}
