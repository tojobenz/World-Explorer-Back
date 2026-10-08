using AuthApi.Domain.Entities;
using AuthApi.Domain.Interfaces;
using AuthApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Infrastructure.Repositories;

/// <summary>
/// Repository for monuments Wikimedia
/// </summary>
public class WikimediaMonumentRepository : GenericRepository<WikimediaMonument>, IWikimediaMonumentRepository
{
    private readonly AppDbContext _context;

    public WikimediaMonumentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<WikimediaMonument?> GetByPageIdAsync(string pageId)
    {
        return await _context.WikimediaMonuments
            .FirstOrDefaultAsync(x => x.PageId == pageId);
    }

    public async Task<IEnumerable<WikimediaMonument>> GetUserFavoritesAsync(Guid userId)
    {
        return await _context.WikimediaMonuments
            .Where(x => x.UserId == userId && x.IsUserFavorite)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<WikimediaMonument?> ToggleFavoriteAsync(Guid userId, string pageId)
    {
        var monument = await _context.WikimediaMonuments
            .FirstOrDefaultAsync(x => x.PageId == pageId);

        if (monument == null)
            return null;

        if (monument.UserId == userId)
        {
            monument.IsUserFavorite = !monument.IsUserFavorite;
        }
        else
        {
            monument.UserId = userId;
            monument.IsUserFavorite = true;
        }

        await _context.SaveChangesAsync();
        return monument;
    }

    public async Task<IEnumerable<WikimediaMonument>> SearchByTypeAsync(string type)
    {
        return await _context.WikimediaMonuments
            .Where(x => x.Type == type)
            .OrderBy(x => x.Title)
            .ToListAsync();
    }
}