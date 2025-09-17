using Domain;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using MockExams.Infra.Database.Mapping;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Database;

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

        modelBuilder.SetUtcOnDatabase();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AtualizarUpdatedAt();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        AtualizarUpdatedAt();
        return base.SaveChanges();
    }

    private void AtualizarUpdatedAt()
    {
        var entries = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}