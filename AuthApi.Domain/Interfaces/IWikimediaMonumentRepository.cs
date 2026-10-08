using AuthApi.Domain.Entities;

namespace AuthApi.Domain.Interfaces;

/// <summary>
/// Interface for repository monuments Wikimedia
/// </summary>
public interface IWikimediaMonumentRepository : IGenericRepository<WikimediaMonument>
{
    Task<WikimediaMonument?> GetByPageIdAsync(string pageId);
    Task<IEnumerable<WikimediaMonument>> GetUserFavoritesAsync(Guid userId);
    Task<WikimediaMonument?> ToggleFavoriteAsync(Guid userId, string pageId);
    Task<IEnumerable<WikimediaMonument>> SearchByTypeAsync(string type);
}