namespace UrlShortener.Entities
{
    public class ShortenedUrl
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string LongUrl { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
