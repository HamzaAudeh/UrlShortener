namespace UrlShortener.Services
{
    public interface IUrlShorteningService
    {
        Task<string> Shorten(string url);
        Task<string> GetFullUrl(string code);
    }
}
