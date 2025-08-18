using FluentValidation;
using System;
using System.Linq;

namespace Domain.Validators;

public class QuestionValidator : AbstractValidator<Question>
{
    private const int MaxTitleLength = 200;
    private const int MaxOptionLength = 300;

    public QuestionValidator()
    {
        RuleFor(q => q.ExamId)
            .NotEmpty().WithMessage("ExamId é obrigatório.");

        RuleFor(q => q.Title)
            .NotEmpty().WithMessage("Campo Título não pode ser vazio.")
            .Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage("Título não pode conter apenas espaços.")
            .MinimumLength(5).WithMessage("Título deve ter pelo menos 5 caracteres.")
            .MaximumLength(MaxTitleLength).WithMessage($"Título deve ter no máximo {MaxTitleLength} caracteres.");

        // Opções obrigatórias mínimas
        RuleFor(q => q.Option1)
            .NotEmpty().WithMessage("Opção 1 é obrigatória.")
            .MaximumLength(MaxOptionLength).WithMessage($"Cada opção deve ter no máximo {MaxOptionLength} caracteres.");
        RuleFor(q => q.Option2)
            .NotEmpty().WithMessage("Opção 2 é obrigatória.")
            .MaximumLength(MaxOptionLength).WithMessage($"Cada opção deve ter no máximo {MaxOptionLength} caracteres.");

        // Demais opções são opcionais, mas se vierem preenchidas, validamos tamanho
        RuleFor(q => q.Option3)
            .MaximumLength(MaxOptionLength).When(q => !string.IsNullOrWhiteSpace(q.Option3))
            .WithMessage($"Cada opção deve ter no máximo {MaxOptionLength} caracteres.");
        RuleFor(q => q.Option4)
            .MaximumLength(MaxOptionLength).When(q => !string.IsNullOrWhiteSpace(q.Option4))
            .WithMessage($"Cada opção deve ter no máximo {MaxOptionLength} caracteres.");
        RuleFor(q => q.Option5)
            .MaximumLength(MaxOptionLength).When(q => !string.IsNullOrWhiteSpace(q.Option5))
            .WithMessage($"Cada opção deve ter no máximo {MaxOptionLength} caracteres.");

        // Pelo menos duas opções preenchidas (1 e 2 já garantem)
        RuleFor(q => q)
            .Must(HaveAtLeastTwoOptions)
            .WithMessage("Informe pelo menos duas opções.");

        // Opções não podem se repetir (ignora maiúsculas/minúsculas e espaços)
        RuleFor(q => q)
            .Must(OptionsAreUnique)
            .WithMessage("As opções não podem se repetir.");

        // Dificuldade: 1..3
        RuleFor(q => q.DifficultyLevel)
            .InclusiveBetween(1, 3)
            .WithMessage("Nível de dificuldade deve ser 1 (Fácil), 2 (Médio) ou 3 (Difícil).");

        // CorrectOptions: "1" ou "1,2" etc, sem duplicados e referenciando opções preenchidas
        RuleFor(q => q.CorrectOptions)
            .NotEmpty().WithMessage("Informe ao menos uma alternativa correta (ex.: '1' ou '1,3').")
            .Must(BeValidCorrectOptions).WithMessage("Formato inválido. Use números de 1 a 5 separados por vírgula (ex.: '1' ou '1,3').");

        RuleFor(q => q)
            .Must(NoDuplicatesInCorrectOptions)
            .WithMessage("As alternativas corretas não podem conter valores repetidos.");

        RuleFor(q => q)
            .Must(CorrectOptionsReferenceFilledOptions)
            .WithMessage("As alternativas corretas devem referenciar opções preenchidas.");
    }

    private static bool HaveAtLeastTwoOptions(Question q)
    {
        int count = 0;
        if (!string.IsNullOrWhiteSpace(q.Option1)) count++;
        if (!string.IsNullOrWhiteSpace(q.Option2)) count++;
        if (!string.IsNullOrWhiteSpace(q.Option3)) count++;
        if (!string.IsNullOrWhiteSpace(q.Option4)) count++;
        if (!string.IsNullOrWhiteSpace(q.Option5)) count++;
        return count >= 2;
    }

    private static bool OptionsAreUnique(Question q)
    {
        var vals = new[] { q.Option1, q.Option2, q.Option3, q.Option4, q.Option5 }
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Select(s => s.ToUpperInvariant())
            .ToList();
        return vals.Count == vals.Distinct().Count();
    }

    private static bool BeValidCorrectOptions(string value)
    {
        var parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0) return false;

        foreach (var p in parts)
        {
            if (!int.TryParse(p, out var idx)) return false;
            if (idx < 1 || idx > 5) return false;
        }
        return true;
    }

    private static bool NoDuplicatesInCorrectOptions(Question q)
    {
        var parts = (q.CorrectOptions ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length == parts.Distinct().Count();
    }

    private static bool CorrectOptionsReferenceFilledOptions(Question q)
    {
        if (string.IsNullOrWhiteSpace(q.CorrectOptions)) return false;

        var filled = new[]
        {
            q.Option1, // 1
            q.Option2, // 2
            q.Option3, // 3
            q.Option4, // 4
            q.Option5  // 5
        };

        var parts = q.CorrectOptions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var p in parts)
        {
            if (!int.TryParse(p, out var idx)) return false;
            if (idx < 1 || idx > 5) return false;
            if (string.IsNullOrWhiteSpace(filled[idx - 1])) return false;
        }
        return true;
    }
}