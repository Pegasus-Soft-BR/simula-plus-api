using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MockExams.Infra.Database.Providers;

public interface IExamSearchProvider
{
    Task<IList<Exam>> SearchAsync(ApplicationDbContext ctx, string termNormalized);
}