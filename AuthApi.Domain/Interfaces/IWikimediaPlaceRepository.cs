using AuthApi.Domain.Entities;

namespace AuthApi.Domain.Interfaces;

/// <summary>
/// Interface repository place Wikimedia
/// </summary>
public interface IWikimediaPlaceRepository : IGenericRepository<WikimediaPlace>
{
    Task<WikimediaPlace?> GetByPageIdAsync(string pageId);
    Task<IEnumerable<WikimediaPlace>> GetUserFavoritesAsync(Guid userId);
    Task<WikimediaPlace?> ToggleFavoriteAsync(Guid userId, string pageId);
    Task<IEnumerable<WikimediaPlace>> SearchByTypeAsync(string type);
}