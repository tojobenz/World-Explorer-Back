using AuthApi.Domain.Entities;
using AuthApi.Domain.Interfaces;
using AuthApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Infrastructure.Repositories;

/// <summary>
/// Repository for places Wikimedia
/// </summary>
public class WikimediaPlaceRepository : GenericRepository<WikimediaPlace>, IWikimediaPlaceRepository
{
    private readonly AppDbContext _context;

    public WikimediaPlaceRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<WikimediaPlace?> GetByPageIdAsync(string pageId)
    {
        return await _context.WikimediaPlaces
            .FirstOrDefaultAsync(x => x.PageId == pageId);
    }

    public async Task<IEnumerable<WikimediaPlace>> GetUserFavoritesAsync(Guid userId)
    {
        return await _context.WikimediaPlaces
            .Where(x => x.UserId == userId && x.IsUserFavorite)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<WikimediaPlace?> ToggleFavoriteAsync(Guid userId, string pageId)
    {
        var place = await _context.WikimediaPlaces
            .FirstOrDefaultAsync(x => x.PageId == pageId);

        if (place == null)
            return null;

        if (place.UserId == userId)
        {
            place.IsUserFavorite = !place.IsUserFavorite;
        }
        else
        {
            place.UserId = userId;
            place.IsUserFavorite = true;
        }

        await _context.SaveChangesAsync();
        return place;
    }

    public async Task<IEnumerable<WikimediaPlace>> SearchByTypeAsync(string type)
    {
        return await _context.WikimediaPlaces
            .Where(x => x.Type == type)
            .OrderBy(x => x.Title)
            .ToListAsync();
    }
}