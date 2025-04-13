using AutoMapper;
using Domain.DTOs.Exam;
using Helper;
using Infra.IA;
using System;
using System.Threading.Tasks;

namespace Service.Exam.Generator;

public class ExamGeneratorService : IExamGeneratorService
{
    private readonly IIAClient _iaClient;
    private readonly IMapper _mapper;

    public ExamGeneratorService(IIAClient iaClient, IMapper mapper)
    {
        _iaClient = iaClient;
        _mapper = mapper;
    }

    public async Task<Domain.Exam> GenerateAsync(string term)
    {
        var prompt = PromptFactory(term);
        var rawJson = await _iaClient.GenerateAsync(prompt);

        var dto = JsonHelper.FromJson<CreateExamDto>(rawJson);

        if (dto == null)
            throw new Exception("Não foi possível gerar o exame via IA.");

        return _mapper.Map<Domain.Exam>(dto);
    }

    private string PromptFactory(string term)
    {
        var totalQuestions = 10;

        var prompt = $$"""
             Você é um especialista em simulados técnicos para desenvolvedores.

             Crie um simulado de {{totalQuestions}} questões de múltipla escolha sobre o tema: '{{term}}'. 

             O formato de saída deve ser um JSON que obedeça à seguinte estrutura exata:

             {
               "title": "Título chamativo do simulado",
               "description": "Resumo breve sobre o tema",
               "questions": [
         	    {

                  "title": "Enunciado da pergunta",
         	      "option1": "Opção A",
         	      "option2": "Opção B",
         	      "option3": "Opção C",
         	      "option4": "Opção D",
         	      "option5": "Opção E",
         	      "correctOptions": "Exemplos válidos '1' ou '1,2'", 
         	      "difficultyLevel": 1 
         	    }
               ]
             }

             Regras:
             - A resposta deve ser **apenas** o JSON, sem texto explicativo antes ou depois.
             - Não inclua blocos Markdown como ```json ou ``` — apenas o conteúdo bruto do JSON.
             - Sempre retorne {{totalQuestions}} questões.
             - Use 1 ou várias alternativas corretas, entre 1 e 5. 
             - As questões devem ter níveis de dificuldade variados.
             - O JSON deve estar completo e válido, sem comentários.
             - O campo "difficultyLevel" deve ser um número inteiro, sem aspas. 1 = Fácil, 2 = Médio, 3 = Difícil

         """;

        return prompt;
    }
}
