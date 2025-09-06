using AutoMapper;
using Domain;
using Domain.Common;
using Domain.DTOs;
using Domain.DTOs.Commom;
using Domain.DTOs.Exam;
using Domain.Enums;
using Domain.Exceptions;
using FluentValidation;
using Infra.PegasusApi;
using Infra.PegasusApi.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockExams.Infra.Database;
using MockExams.Infra.Database.UoW;
using MockExams.Service.Generic;
using Service.Exam.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockExams.Service;

public class ExamService : BaseService<Exam, ExamDto>, IExamService
{
    private readonly IExamGeneratorService _generator;
    private readonly IPegasusApiClient _pegasusApiClient;
    protected ILogger<ExamService> _logger;


    public ExamService(ApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IValidator<Exam> validator, IMapper mapper,
        IExamGeneratorService generator, ILogger<ExamService> logger, IPegasusApiClient pegasusApiClient) : base(context, unitOfWork, validator, mapper)
    {
        _generator = generator;
        _logger = logger;
        _pegasusApiClient = pegasusApiClient;
    }

    public StartExamAttemptDto StartExamAttempt(Guid? userId, Guid examId)
    {
        // 1 - validar
        if (!userId.HasValue)
            throw new BizException(BizException.Error.NotAuthorized);

        var exam = _ctx.Exams.FirstOrDefault(e => e.Id == examId);
        if (exam == null)
            throw new BizException(BizException.Error.NotFound, $"Não encontramos o exame '{examId.ToString()}'.");

        // 2 - criar tentativa de exame
        var attempt = new ExamAttempt()
        {
            UserId = userId.Value,
            ExamId = examId,

        };

        // 3 - sortear n questões e popular a lista de respostas vazias
        var randomQuestions = _ctx.Questions
            .Where(q => q.ExamId == examId)
            .OrderBy(q => Guid.NewGuid()) // Ordena aleatoriamente
            .Take(exam.TotalQuestionsPerAttempt) // Seleciona N perguntas
            .ToList();

        attempt.Answers = randomQuestions.Select(q => new Answer()
        {
            QuestionId = q.Id,
            ExamAttemptId = attempt.Id
        }).ToList();

        _ctx.ExamAttempts.Add(attempt);
        _ctx.SaveChanges();

        var startDto = new StartExamAttemptDto()
        {
            Id = attempt.Id,
            ExamId = examId,
            UserId = userId.Value,
            Questions = _mapper.Map<IList<QuestionDto>>(randomQuestions)
        };

        return startDto;
    }

    public ExamAttemptDto FinishExamAttempt(Guid? userId, FinishExamAttemptDto finishDto)
    {
        // 1 - validar
        if (!userId.HasValue)
            throw new BizException(BizException.Error.NotAuthorized);

        var attempt = _ctx.ExamAttempts.FirstOrDefault(a => a.Id == finishDto.ExamAttemptId);
        if (attempt == null)
            throw new BizException(BizException.Error.NotFound, $"Não encontramos a tentativa de exame '{finishDto.ExamAttemptId.ToString()}'.");

        if (attempt.Status == ExamAttemptStatus.Completed)
            throw new BizException(BizException.Error.BadRequest, "Esta tentativa de exame já foi finalizada.");

        if (userId != attempt.UserId)
            throw new BizException(BizException.Error.Forbidden, "Você não tem permissão para finalizar esta tentativa de exame porque não é sua.");

        // 2 - popular respostas
        foreach (var answer in attempt.Answers)
        {
            var answerDto = finishDto.Answers.FirstOrDefault(a => a.QuestionId == answer.QuestionId);
            if (answerDto == null)
                throw new BizException(BizException.Error.BadRequest, $"Não encontramos a resposta para a questão '{answerDto.QuestionId.ToString()}'.");

            answer.SelectedOptions = answerDto.SelectedOptions;
            answer.IsCorrect = answer.SelectedOptions == answer.Question.CorrectOptions;
        }

        // 3 - calcular resultado
        attempt.Status = ExamAttemptStatus.Completed;
        attempt.FinishedAt = DateTime.UtcNow;
        attempt.TimeSpentSeconds = (int)(attempt.FinishedAt - attempt.CreatedAt).Value.TotalSeconds;

        var totalCorrect = attempt.Answers.Count(a => (bool)a.IsCorrect);
        var totalQuestions = attempt.Answers.Count();
        attempt.Score = (int)((totalCorrect / (double)totalQuestions) * 100);

        _ctx.SaveChanges();

        var attemptDto = _mapper.Map<ExamAttemptDto>(attempt);
        return attemptDto;
    }

    public async Task<PagedList<MyExamAttemptDto>> MyExamAttemptsAsync(Guid? userId, int itemsPerPage = 10, int page = 1)
    {
        if (!userId.HasValue)
            throw new BizException(BizException.Error.NotAuthorized);

        if (itemsPerPage <= 0 || itemsPerPage > MaxItemsPerPage)
            throw new BizException(BizException.Error.BadRequest, $"itemsPerPage deve ser um valor entre 1 e {MaxItemsPerPage}.");

        var query = _ctx.ExamAttempts
            .AsNoTracking()
            .Include(a => a.Exam)
            .Where(a => a.UserId == userId);

        var totalItems = await query.CountAsync();

        var attempts = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .ToListAsync();

        var attemptsDto = _mapper.Map<IList<MyExamAttemptDto>>(attempts);

        return new PagedList<MyExamAttemptDto>
        {
            Items = attemptsDto,
            ItemsPerPage = itemsPerPage,
            Page = page,
            TotalItems = totalItems
        };
    }


    public MyExamAttemptDetailsDto MyExamAttemptDetails(Guid? userId, Guid attemptId)
    {
        if (!userId.HasValue)
            throw new BizException(BizException.Error.NotAuthorized);

        var attempt = _ctx.ExamAttempts
            .Include(a => a.Exam)
            .Include(a => a.Answers)
                .ThenInclude(ans => ans.Question)
            .FirstOrDefault(a => a.Id == attemptId && a.UserId == userId);

        if (attempt == null)
            throw new BizException(BizException.Error.NotFound);

        var questionsWithAnswers = _mapper.Map<List<QuestionWithAnswerDto>>(attempt.Answers);
        var result = _mapper.Map<MyExamAttemptDetailsDto>(attempt.Exam);

        result.Id = attempt.Id;
        result.CreatedAt = attempt.CreatedAt;
        result.TimeSpentSeconds = attempt.TimeSpentSeconds;
        result.Score = attempt.Score;
        result.FinishedAt = attempt.FinishedAt;
        result.QuestionWithAnswer = questionsWithAnswers;
        result.Status = attempt.Status;

        return result;
    }

    public async Task<List<ExamDto>> Search(string term = "", UserDto? currentUser = null)
    {
        if (string.IsNullOrEmpty(term) || term.Length < 2)
            throw new BizException(BizException.Error.BadRequest, "Favor refinar mais o termo de pesquisa.");

        var exams = await FullTextSearch(term);

        if (!exams.Any())
        {
            try
            {
                _logger.LogInformation($"Nenhum exame encontrado com o termo '{term}'. Tentando criar um novo usando IA.");

                var newExam = await _generator.GenerateAsync(term);
                newExam.TimeSpentMaxSeconds = 600; // 10 minutos
                newExam.TotalQuestionsPerAttempt = 5;
                _ctx.Exams.Add(newExam);
                _ctx.SaveChanges();
                await NotifyAdminsOfGeneratedExamAsync(term, currentUser, newExam);

                exams.Add(newExam);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar exame via IA.");
            }

        }

        var examsDto = _mapper.Map<List<ExamDto>>(exams);
        return examsDto;
    }

    private async Task NotifyAdminsOfGeneratedExamAsync(string term, UserDto currentUser, Exam newExam)
    {
        // Notificar admins sobre simulado gerado por IA
        try
        {
            var userInfo = currentUser != null && !string.IsNullOrEmpty(currentUser.Name)
                ? $"{currentUser.Name} ({currentUser.Email})"
                : "Usuário anônimo";

            var message = $@"<strong>🤖 Novo simulado gerado por IA</strong>
                <ul>
                <li><strong>Título:</strong> {newExam.Title}</li>
                <li><strong>Termo pesquisado:</strong> {term}</li>
                <li><strong>Solicitado por:</strong> {userInfo}</li>
                <li><strong>Total de questões:</strong> {newExam.Questions?.Count ?? 0}</li>
                </ul>";

            await _pegasusApiClient.NotifyAdminsAsync(new AdminNotificationRequest
            {
                App = "mock-exams",
                Name = "Simula+ (Sistema)",
                Email = "noreply@pegasus-soft.com.br",
                Phone = "22 988317391",
                Business = "",
                Message = message,
                Subject = "🤖 Novo simulado gerado por IA"
            });

            _logger.LogInformation("Admins notificados sobre simulado gerado por IA: {ExamTitle} por usuário {UserId}", newExam.Title, currentUser?.Id);
        }
        catch (Exception notifyEx)
        {
            _logger.LogError(notifyEx, "Erro ao notificar admins sobre simulado gerado por IA: {ExamTitle}", newExam.Title);
        }
    }

    private async Task<List<Exam>> FullTextSearch(string term)
    {
        term = term.ToLower().Trim();

        var keywords = term
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(k => k.Length >= 3)
            .Select(k => $"\"{k}*\"");

        var containsClause = string.Join(" AND ", keywords);

        var sql = $@"
        SELECT TOP 10 e.*
        FROM CONTAINSTABLE(Exams, (Title, Description), '{containsClause}') AS ft
        JOIN Exams e ON e.Id = ft.[KEY]
        ORDER BY ft.[RANK] DESC";

        return await _ctx.Exams
            .FromSqlRaw(sql)
            .ToListAsync();
    }
}