using System;
using System.Collections.Generic;
using Domain;
using Domain.Common;
using System.Diagnostics;
using Rollbar;
using Microsoft.EntityFrameworkCore;
using MockExams.Infra.Database;

namespace MockExams.Jobs
{
    // inspirado no design pattern chain of responsability
    // https://pt.wikipedia.org/wiki/Chain_of_ResponMockExamsility

    public class JobExecutor : IJobExecutor
    {
        private readonly IList<IJob> _jobs;
        private readonly DbSet<JobHistory> _jobHistoryRepo;
        private readonly ApplicationDbContext _ctx;
        private Stopwatch _stopwatch;

        public JobExecutor(ApplicationDbContext context,
                           ExampleJob job1)
        {
            _ctx = context;
            _jobHistoryRepo = context.JobHistories;

            _jobs = new List<IJob>
            {
                job1
            };

        }

        public JobExecutorResult Execute()
        {
            _stopwatch = Stopwatch.StartNew();

            var messages = new List<string>();
            var success = true;

            try
            {
                foreach (IJob job in _jobs)
                {
                    if (!job.Active)
                    {
                        messages.Add(string.Format("Job {0}: job não foi executado porque está INATIVO.", job.JobName));
                        continue;
                    }

                    if (job.HasWork())
                    {

                        if (job.Execute())
                        {
                            messages.Add(string.Format("Job {0}: job executado com sucesso.", job.JobName));
                        }
                        else
                        {
                            success = false;
                            messages.Add(string.Format("Job {0}: ocorreu um erro ao executar o job. Verifique os logs.", job.JobName));
                        }

                        // executa apenas um job por ciclo.
                        break;
                    }
                    else
                    {
                        messages.Add(string.Format("Job {0}: não tinha nenhum trabalho a ser feito.", job.JobName));
                    }
                }
            }
            catch(Exception ex)
            {
                success = false;
                messages.Add(string.Format("Executor: ocorreu um erro fatal. {0}", ex.Message));
                SendErrorToRollbar(ex);
            }

            // Executor também loga seu histórico. Precisamos de rastreabilidade.
            _stopwatch.Stop();
            var details = String.Join("\n", messages.ToArray());
            LogExecutorAddHistory(success, details);

            return new JobExecutorResult()
            {
                Success = success,
                Messages = messages
            };
        }

        private void LogExecutorAddHistory(bool success, string details)
        {
            var history = new JobHistory()
            {
                JobName = "JobExecutor",
                IsSuccess = success,
                Details = details,
                TimeSpentSeconds = ((double)_stopwatch.ElapsedMilliseconds / (double)1000),
            };

            _jobHistoryRepo.Add(history);
            _ctx.SaveChanges();
        }

        // TODO: criar um service pro rollbar e reaproveitar aqui
        // e no ExceptionHandlerMiddleware.
        private void SendErrorToRollbar(Exception ex)
        {
            object error = new
            {
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Source = ex.Source
            };

            RollbarLocator.RollbarInstance.Error(error);
        }
    }
}
