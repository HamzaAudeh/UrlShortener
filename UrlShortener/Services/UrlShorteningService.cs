using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using UrlShortener.Data;

namespace UrlShortener.Services
{
    public class UrlShorteningService : IUrlShorteningService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlShorteningService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Shorten(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            string code = await GenerateCode(url);

            var request = _httpContextAccessor.HttpContext.Request;
            var entity = new Entities.ShortenedUrl()
            {
                Code = code,
                LongUrl = url,
                ShortUrl = $"{request.Scheme}://{request.Host}/{code}",
                UserId = 1, // for now it's static
                CreatedOnUtc = DateTime.UtcNow
            };

            _context.ShortenedUrls.Add(entity);
            await _context.SaveChangesAsync();
            return entity.ShortUrl;
        }

        public async Task<string> GetFullUrl(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var entity = await _context.ShortenedUrls.FirstOrDefaultAsync(c => c.Code == code);
            if (entity is null)
            {
                throw new Exception("Entity not found");
            }

            return entity.LongUrl;
        }

        private async Task<string> GenerateCode(string url)
        {
            byte[] hash;
            using (var sha256 = SHA256.Create())
            {
                hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(url));
            }

            // Step 2: Encode the hash using Base62
            string base62Hash = Base62Encoder.Encode(hash);

            // Step 3: Truncate the encoded string to 8 characters
            string code = base62Hash[..8];

            // Step 4: Check for collision and resolve if necessary
            int counter = 0;
            while (await DoesCodeExist(code))
            {
                // Modify the short URL slightly to avoid collision
                code = base62Hash[..(8 - counter.ToString().Length)] + counter;
                counter++;
            }

            return code;
        }

        private async Task<bool> DoesCodeExist(string code)
        {
            bool doesExist = await _context.ShortenedUrls.AnyAsync(x => x.Code == code);
            return doesExist;
        }
    }

    public static class Base62Encoder
    {
        private const string Base62Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly int Base62Length = Base62Chars.Length;

        public static string Encode(byte[] input)
        {
            var sb = new StringBuilder();
            var dividend = new BigInteger(input.Concat(new byte[] { 0 }).ToArray());
            while (dividend > 0)
            {
                dividend = BigInteger.DivRem(dividend, Base62Length, out var remainder);
                sb.Insert(0, Base62Chars[(int)remainder]);
            }
            return sb.ToString();
        }
    }
}
