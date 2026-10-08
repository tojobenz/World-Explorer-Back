using AuthApi.Domain.Common;

namespace AuthApi.Domain.Entities;

/// <summary>
/// Entity personality Wikimedia
/// </summary>
public class WikimediaPerson : BaseEntity
{
    public string PageId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Extract { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? FullUrl { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string? BirthPlace { get; set; }
    public string? Nationality { get; set; }
    public string? Occupation { get; set; }
    public bool IsUserFavorite { get; set; } = false;
    public Guid? UserId { get; set; }
    public User? User { get; set; }
}