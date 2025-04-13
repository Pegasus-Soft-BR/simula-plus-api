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

    public DbSet<User> Users { get; set; }
    public DbSet<LogEntry> LogEntries { get; set; }
    public DbSet<JobHistory> JobHistories { get; set; }
    public DbSet<AccessHistory> AccessHistories { get; set; }

    public DbSet<Exam> Exams { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<ExamAttempt> ExamAttempts { get; set; }
    public DbSet<Answer> Answers { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new AnswerMap(modelBuilder.Entity<Answer>());

        base.OnModelCreating(modelBuilder);

        new UserMap(modelBuilder.Entity<User>());
        new JobHistoryMap(modelBuilder.Entity<JobHistory>());
        new LogEntryMap(modelBuilder.Entity<LogEntry>());

        this.SetUtcOnDatabase(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await this.LogChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        this.LogChanges().Wait();
        return base.SaveChanges();
    }
}
