using System.Collections.Generic;

namespace Infra.PegasusApi.Dtos;

public class AdminNotificationResponse
{
    public List<string> ErrorMessages { get; set; } = new List<string>();
    public string SuccessMessage { get; set; } = string.Empty;
    public bool Success { get; set; }
}