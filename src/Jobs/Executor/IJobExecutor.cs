using System;
using Domain.Common;

namespace MockExams.Jobs
{
    public interface IJobExecutor
    {
        JobExecutorResult Execute();
    }
}
