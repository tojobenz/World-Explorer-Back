using AuthApi.Domain.Common;

namespace AuthApi.Domain.Entities;

/// <summary>
/// Entity for fact historic Wikimedia
/// </summary>
public class WikimediaFact : BaseEntity
{
    public string PageId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Extract { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? FullUrl { get; set; }
    public DateTime? EventDate { get; set; }
    public string? Location { get; set; }
    public string? Category { get; set; } // "war", "politics", "science", etc.
    public string? RelatedPeople { get; set; } // JSON array of related person names
    public bool IsUserFavorite { get; set; } = false;
    public Guid? UserId { get; set; }
    public User? User { get; set; }
}