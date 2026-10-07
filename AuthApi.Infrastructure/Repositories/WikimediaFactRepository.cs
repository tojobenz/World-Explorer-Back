using AuthApi.Domain.Entities;
using AuthApi.Domain.Interfaces;
using AuthApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Infrastructure.Repositories;

/// <summary>
/// Repository for facts Wikimedia
/// </summary>
public class WikimediaFactRepository : GenericRepository<WikimediaFact>, IWikimediaFactRepository
{
    private readonly AppDbContext _context;

    public WikimediaFactRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<WikimediaFact?> GetByPageIdAsync(string pageId)
    {
        return await _context.WikimediaFacts
            .FirstOrDefaultAsync(x => x.PageId == pageId);
    }

    public async Task<IEnumerable<WikimediaFact>> GetUserFavoritesAsync(Guid userId)
    {
        return await _context.WikimediaFacts
            .Where(x => x.UserId == userId && x.IsUserFavorite)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<WikimediaFact?> ToggleFavoriteAsync(Guid userId, string pageId)
    {
        var fact = await _context.WikimediaFacts
            .FirstOrDefaultAsync(x => x.PageId == pageId);

        if (fact == null)
            return null;

        if (fact.UserId == userId)
        {
            fact.IsUserFavorite = !fact.IsUserFavorite;
        }
        else
        {
            fact.UserId = userId;
            fact.IsUserFavorite = true;
        }

        await _context.SaveChangesAsync();
        return fact;
    }

    public async Task<IEnumerable<WikimediaFact>> SearchByCategoryAsync(string category)
    {
        return await _context.WikimediaFacts
            .Where(x => x.Category == category)
            .OrderByDescending(x => x.EventDate)
            .ToListAsync();
    }
}