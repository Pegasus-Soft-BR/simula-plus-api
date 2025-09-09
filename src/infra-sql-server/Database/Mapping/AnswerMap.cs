using Domain;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MockExams.Infra.Database.Mapping;

public class AnswerMap
{
    public AnswerMap(EntityTypeBuilder<Answer> entityBuilder)
    {
        entityBuilder.HasKey(t => t.Id);

        entityBuilder.HasOne(a => a.Question)
            .WithMany()
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);

        // O SQL Server não permite múltiplas ON DELETE CASCADE que possam causar dependências cíclicas.
        // por isso usamos SetNull para evitar a exceção de dependência cíclica
    }
}