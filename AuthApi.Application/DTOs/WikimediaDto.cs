namespace AuthApi.Application.DTOs;

/// <summary>
/// DTO result search wikimedia
/// </summary>
public class WikimediaSearchResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<WikimediaItemDto>? Data { get; set; }
}

/// <summary>
/// DTO  item Wikimedia (place , person, monument, fact)
/// </summary>
public class WikimediaItemDto
{
    public string PageId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Extract { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? FullUrl { get; set; }
    public string? Category { get; set; } // "place", "person", "monument", "fact"
    public List<string>? Coordinates { get; set; } // [latitude, longitude]
    public DateTime? CreatedAt { get; set; }
    public bool IsFavorite { get; set; } = false;

}

/// <summary>
/// DTO pour les paramètres de recherche Wikimedia
/// </summary>
public class WikimediaSearchRequestDto
{
    public string Query { get; set; } = string.Empty;
    public string? Category { get; set; } // "place", "person", "monument", "fact"
    public int Limit { get; set; } = 10;
    public string? Language { get; set; } = "fr";
}

/// <summary>
/// DTO detail
/// </summary>
public class WikimediaDetailDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public WikimediaItemDto? Data { get; set; }
}