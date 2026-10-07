using AuthApi.Domain.Entities;

namespace AuthApi.Domain.Interfaces;

/// <summary>
/// Interface  repository personality Wikimedia
/// </summary>
public interface IWikimediaPersonRepository : IGenericRepository<WikimediaPerson>
{
    Task<WikimediaPerson?> GetByPageIdAsync(string pageId);
    Task<IEnumerable<WikimediaPerson>> GetUserFavoritesAsync(Guid userId);
    Task<WikimediaPerson?> ToggleFavoriteAsync(Guid userId, string pageId);
    Task<IEnumerable<WikimediaPerson>> SearchByOccupationAsync(string occupation);
    Task<IEnumerable<WikimediaPerson>> SearchByNationalityAsync(string nationality);
}