using Domain.Common;
using System;

namespace Domain.DTOs;

public abstract class BaseDTO : IIdProperty
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
