using AuthApi.Domain.Common;

namespace AuthApi.Domain.Entities;

/// <summary>
/// Entity for monument Wikimedia
/// </summary>
public class WikimediaMonument : BaseEntity
{
    public string PageId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Extract { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? FullUrl { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public DateTime? ConstructionYear { get; set; }
    public string? Architect { get; set; }
    public string? Type { get; set; } // "castle", "church", "statue", etc.
    public bool IsUserFavorite { get; set; } = false;
    public Guid? UserId { get; set; }
    public User? User { get; set; }
}