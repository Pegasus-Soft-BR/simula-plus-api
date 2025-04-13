using System;
using System.Diagnostics;
using System.Linq;
using Domain;
using Domain.Enums;
using MockExams.Helper;
using Microsoft.EntityFrameworkCore;
using MockExams.Infra.Database;

namespace MockExams.Jobs
{
    public class GenericJob
    {
        public string JobName { get; set; }
        public string Description { get; set; }
        public Interval Interval { get; set; }
        public bool Active { get; set; }
        public TimeSpan? BestTimeToExecute { get; set; }

        protected readonly DbSet<JobHistory> _jobHistoryRepo;

        protected readonly ApplicationDbContext _ctx;

        protected Stopwatch _stopwatch;

        public GenericJob(ApplicationDbContext context)
        {
            _ctx = context;
            _jobHistoryRepo = _ctx.JobHistories;
        }

        public bool HasWork()
        {
            if(BestTimeToExecute != null)
            {
                var timeNow = DateTimeHelper.GetTimeNowSaoPaulo();
                if (timeNow < BestTimeToExecute) return false;
            }

            var DateLimit = GetDateLimitByInterval(Interval);

            var hasHistory =
            _jobHistoryRepo
            .Where(x => x.CreatedAt > DateLimit &&
                   x.JobName == JobName &&
                   x.IsSuccess == true)
            .ToList().Any();

            return !hasHistory;
        }

        public DateTime GetDateLimitByInterval(Interval i)
        {
            var result = DateTime.UtcNow;

            switch (i)
            {
                case Interval.Dayly:
                {
                    result = DateTimeHelper.GetTodaySaoPaulo();
                    break;
                }
                case Interval.Hourly:
                {
                    result = result.AddHours(-1);
                    break;
                }
                case Interval.Weekly:
                {
                    result = result.AddDays(-7);
                    break;
                }
                case Interval.Each30Minutes:
                {
                    result = result.AddMinutes(-30);
                    break;
                }
                case Interval.Each5Minutes:
                {
                    result = result.AddMinutes(-5);
                    break;
                }
            }

            // ajuste de +1 minuto, levando em consideração o tempo que o job
            // pode precisar para completar em sua última execução.
            result = result.AddMinutes(+1);
            return result;
        }

        public bool Execute()
        {
            BeforeWork();
            var history = Work();
            AfterWork(history);

            return history.IsSuccess;
        }

        // Sempre sobrescrito pelo Job real.
        public virtual JobHistory Work() => new JobHistory();

        protected void BeforeWork()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        protected void AfterWork(JobHistory history)
        {
            _stopwatch.Stop();

            history.TimeSpentSeconds = ((double)_stopwatch.ElapsedMilliseconds / (double)1000); ;
            _jobHistoryRepo.Add(history);
            _ctx.SaveChanges();
        }

    }
}
