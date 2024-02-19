using System.Text.RegularExpressions;

namespace MoviesDb.Domain.Models;

public class Movie
{
    public required Guid Id { get; set; }
    public string Slug => GenerateSlug();
    public required string Title { get; set; }
    public required int YearOfRelease { get; set; }
    public List<string> Genres { get; set; } = new();
    public DateTimeOffset  CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    private string GenerateSlug()
    {
        var sluggedTitle = Regex.Replace(Title, "[^0-9A-Za-z _-]", string.Empty)
            .ToLower()
            .Replace(" ", "-");
        return $"{sluggedTitle}-{YearOfRelease}";
    }
}
