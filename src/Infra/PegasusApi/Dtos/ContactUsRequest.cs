namespace Infra.PegasusApi.Dtos;

public class ContactUsRequest
{
    public string App { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Business { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}