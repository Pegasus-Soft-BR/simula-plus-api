using Domain;
using Domain.Enums;
using MockExams.Infra.Database;
using MockExams.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MockExams.Jobs
{
    public class ExampleJob : GenericJob, IJob
    {
        public ExampleJob(
            ApplicationDbContext context
            ) : base(context)
        {
            JobName = "SetLoanLate";
            Description = "Busca os empréstimos em atraso e atualiza status = Atrasado.";
            Interval = Interval.Dayly;
            Active = true;
            BestTimeToExecute = new TimeSpan(6, 0, 0);
        }

        public override JobHistory Work()
        {
            // TODO: add you logic here

            return new JobHistory()
            {
                JobName = JobName,
                IsSuccess = true,
                Details = "Exemplo de job."
            };

        }
    }
}
