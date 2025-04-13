using Domain;
using Domain.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Domain.Validators;

public class ExamValidator : AbstractValidator<Exam>
{
    public ExamValidator()
    {
        RuleFor(u => u.Title)
           .NotEmpty()
           .WithMessage("Campo título não pode ser vazio.");

        RuleFor(u => u.Description)
            .NotEmpty()
            .WithMessage("Campo Descrição não pode ser vazio.");
    }
}
