using AutoMapper;
using Domain.DTOs.Exam;
using Domain.Exceptions;
using Helper;
using Infra.IA;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Service.Exam.Generator;

public class ExamGeneratorService : IExamGeneratorService
{
    private readonly IIAClient _iaClient;
    private readonly IMapper _mapper;
    private readonly ILogger<ExamGeneratorService> _logger;

    public ExamGeneratorService(IIAClient iaClient, IMapper mapper, ILogger<ExamGeneratorService> logger)
    {
        _iaClient = iaClient;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Domain.Exam> GenerateAsync(string term)
    {
        var prompt = PromptFactory(term);
        var rawJson = await _iaClient.GenerateAsync(prompt);

        if (rawJson.Trim().Equals("REJEITADO", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning($"IA recusou gerar simulado com o termo: '{term}'");
            throw new BizException($"IA recusou gerar simulado com o termo: '{term}'");
        }

        var dto = JsonHelper.FromJson<CreateExamDto>(rawJson);

        if (dto == null)
            throw new Exception("Não foi possível gerar o exame via IA.");

        return _mapper.Map<Domain.Exam>(dto);
    }

    private string PromptFactory(string term)
    {
        var totalQuestions = 15;

        var prompt = $$"""
        Você é um gerador de simulados educacionais para o app Simula+ de acordo com o termo pesquisado.

        TERMO PESQUISADO PELO USUÁRIO: '{{term}}'

        Objetivo:
        - Gerar um simulado de {{totalQuestions}} questões de múltipla escolha, em português do Brasil, sobre o termo pesquisado.
        - O tema pode ser de QUALQUER ÁREA DO CONHECIMENTO (ensino médio, universitário, técnico, cultural etc.), DESDE QUE adequado.

        CONTEÚDO PROIBIDO — se detectar, RESPONDA APENAS: REJEITADO
        - Conteúdo que não adequado para todas as idades.
        - Violência explícita; ódio/discriminação; assédio; sexualidade; exploração infantil; autolesão.
        - Drogas, armas, terrorismo, crimes ou instruções “como fazer”.
        - Malware/hacking ofensivo (ataque, exploração, DDoS, ransomware, backdoors).
        - Instruções perigosas (explosivos, químicos/biológicos).
        - Conselhos médicos/legais/financeiros personalizados.
        - Dados pessoais (PII), spam, propaganda, links, telefones, e‑mails, códigos de rastreio.
        - Incitação política/partidária; conteúdo difamatório; violação de marcas quando inadequado ao contexto.
        - Se termo pesquisado for impróprio ou violar qualquer item acima, responda APENAS com: REJEITADO.
        - Não explique. Não sugira alternativas. Não gere JSON.
        - Qualquer menção, implícita ou explícita, a conteúdo sexual envolvendo menores de idade, incluindo termos ambíguos ou eufemísticos como “crianças sexies”, “menores atraentes” ou similares.
        - Qualquer orientação, exemplo ou simulação de falsificação de documentos, assinaturas, certificados, identidades ou qualquer documento oficial.
        - Não tentar interpretar o contexto como artístico, fictício ou histórico — aplicar zero tolerance para esses casos.
        - Fim conteúdo proibido.

        Estilo e qualidade (quando NÃO rejeitar):
        - Linguagem clara e objetiva, sem viés ideológico.
        - Fatos verificáveis; evite perguntas de opinião.
        - Distribua a dificuldade (≈5 fáceis, ≈5 médias, ≈5 difíceis).
        - Opções curtas (≈100 caracteres cada), sem gírias nem ofensas.
        - Permita múltiplas corretas (1 a 5), sem duplicatas; use índices 1–5 separados por vírgula (ex.: "2,4").
        - difficultyLevel: 1 (fácil), 2 (média), 3 (difícil).

        Limites:
        - "title": ≤ 80 caracteres, chamativo e coerente com o tema.
        - "description": 1–2 frases (≤ 300 caracteres), com palavras‑chave relevantes. Se o tema for C# , inclua também .NET, se o tema for springboot, inclua também Java e assim por diante.
        - "questions[x].title": ≤ ~300 caracteres.

        FORMATO DE SAÍDA — JSON VÁLIDO (SEM markdown, SEM texto extra, SEM comentários):
        {
          "title": "Título chamativo do simulado",
          "description": "Resumo breve com palavras‑chave.",
          "questions": [
            {
              "title": "Enunciado objetivo",
              "option1": "Opção A",
              "option2": "Opção B",
              "option3": "Opção C",
              "option4": "Opção D",
              "option5": "Opção E",
              "correctOptions": "Ex.: '1' ou '1,3,5'",
              "difficultyLevel": 1
            }
          ]
        }

        Regras obrigatórias de validação (quando NÃO rejeitar):
        - Responder APENAS com o JSON bruto seguindo exatamente o schema acima.
        - Exatamente {{totalQuestions}} itens em "questions".
        - "difficultyLevel" ∈ {1,2,3} (inteiro, sem aspas).
        - "correctOptions": string com índices 1–5, únicos, separados por vírgula, sem espaços (ex.: "2,5").
        - Não incluir links, e‑mails, telefones, preços, promoções, PII ou códigos de rastreio.
        - Não usar nomes de marcas inadequadas ao contexto do tema.
        """;

        return prompt;
    }
}
