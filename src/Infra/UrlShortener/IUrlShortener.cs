namespace MockExams.Infra.UrlShortener;

public interface IUrlShortener
{
    string GetShortUrl(string longUrl);
}