using System;

namespace Domain.DTOs;

public class AccessHistoryDto {

    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public string VisitorName { get; private set; } // Quem acessou o dado?
    public string Profile { get; private set; } // Qual o cargo dele?
}