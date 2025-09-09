using Domain;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using MockExams.Infra.Database.Mapping;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MockExams.Infra.Database;

// ----------------------------------------------------------------------------------
// Esse contexto faz parte de uma arquitetura multi-provider onde suportamos
// SQL Server, PostgreSQL e SQLite usando o mesmo nome de classe e namespace.
//
// Todos os ApplicationDbContext vivem em projetos separados:
// - infra-sql-server
// - infra-postgres  
// - infra-sqlite
//
// A seleção de qual contexto será usado é feita via configuração no appsettings.json
// através do DatabaseConfiguration.cs que faz o switch baseado no DatabaseProvider.
//
// Isso permite que a aplicação use sempre `ApplicationDbContext` em toda parte,
// sem precisar conhecer o banco usado ou depender de interfaces extras.
//
// Simples, previsível e fácil de debugar.
// ----------------------------------------------------------------------------------

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