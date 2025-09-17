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
    }
}