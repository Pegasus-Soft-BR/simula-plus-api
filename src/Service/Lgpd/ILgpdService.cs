using Domain;
using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MockExams.Lgpd;

public interface ILgpdService
{
    public void Anonymize(UserAnonymizeDto dto);
    Task<IList<AccessHistory>> GetWhoAccessedMyProfile(Guid userId);
}
