using AuthApi.Domain.Entities;

namespace AuthApi.Domain.Interfaces;

/// <summary>
/// Interface forrepository fact Wikimedia
/// </summary>
public interface IWikimediaFactRepository : IGenericRepository<WikimediaFact>
{
    Task<WikimediaFact?> GetByPageIdAsync(string pageId);
    Task<IEnumerable<WikimediaFact>> GetUserFavoritesAsync(Guid userId);
    Task<WikimediaFact?> ToggleFavoriteAsync(Guid userId, string pageId);
    Task<IEnumerable<WikimediaFact>> SearchByCategoryAsync(string category);
}