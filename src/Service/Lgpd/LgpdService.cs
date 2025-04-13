using Microsoft.EntityFrameworkCore;
using Domain;
using Domain.DTOs;
using Domain.Enums;
using Domain.Exceptions;
using MockExams.Infra.Database;
using MockExams.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockExams.Lgpd;

public class LgpdService: ILgpdService
{
    private readonly IUserService _userService;
    private readonly IUserEmailService _userEmailService;
    private readonly ApplicationDbContext _ctx;

    public LgpdService(
        IUserService userService,
        IUserEmailService userEmailService,
        ApplicationDbContext context)
    {
        _userService = userService;
        _userEmailService = userEmailService;
        _ctx = context;
    }

    public void Anonymize(UserAnonymizeDto dto)
    {
        var user = _ctx.Users
            .Where(u => u.Id == dto.UserId)
            .FirstOrDefault();

        if (user == null)
            throw new BizException(BizException.Error.NotFound, "Nenhum usuário encontrado.");

        if (!_userService.IsValidPassword(user, dto.Password))
            throw new BizException(BizException.Error.BadRequest, "Senha inválida.");

        if (string.IsNullOrEmpty(dto.Reason))
            throw new BizException(BizException.Error.BadRequest, "Favor informar a justificativa.");

        if (!user.Active)
            throw new BizException(BizException.Error.BadRequest, "Essa conta não está ativa.");

        // 1 - Cancela os simulados em aberto.
        // TODO
        

        // 2 - Anonimiza a conta
        user.Anonymize();

        // 3 - Exclui os logs.
        RemoveLogs(user);

        // 4 - Notifica os adms.
        _userEmailService.SendEmailAnonymizeNotifyAdms(dto);

        // 5 - Enfim salva
        _ctx.SaveChanges();

    }

    public async Task<IList<AccessHistory>> GetWhoAccessedMyProfile(Guid userId)
    {
        return await _ctx.AccessHistories.Where(a => a.UserId == userId).ToListAsync();
    }

    private void RemoveLogs(User user)
    {
        var logsUser = _ctx.LogEntries.Where(log => log.EntityName == "User" && log.EntityId == user.Id).ToArray();
        _ctx.LogEntries.RemoveRange(logsUser);
    }

}
