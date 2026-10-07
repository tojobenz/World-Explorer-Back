using AuthApi.Domain.Entities;
using AuthApi.Domain.Interfaces;
using AuthApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Infrastructure.Repositories;

/// <summary>
/// Repository for personality Wikimedia
/// </summary>
public class WikimediaPersonRepository : GenericRepository<WikimediaPerson>, IWikimediaPersonRepository
{
    private readonly AppDbContext _context;

    public WikimediaPersonRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<WikimediaPerson?> GetByPageIdAsync(string pageId)
    {
        return await _context.WikimediaPeople
            .FirstOrDefaultAsync(x => x.PageId == pageId);
    }

    public async Task<IEnumerable<WikimediaPerson>> GetUserFavoritesAsync(Guid userId)
    {
        return await _context.WikimediaPeople
            .Where(x => x.UserId == userId && x.IsUserFavorite)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<WikimediaPerson?> ToggleFavoriteAsync(Guid userId, string pageId)
    {
        var person = await _context.WikimediaPeople
            .FirstOrDefaultAsync(x => x.PageId == pageId);

        if (person == null)
            return null;

        if (person.UserId == userId)
        {
            person.IsUserFavorite = !person.IsUserFavorite;
        }
        else
        {
            person.UserId = userId;
            person.IsUserFavorite = true;
        }

        await _context.SaveChangesAsync();
        return person;
    }

    public async Task<IEnumerable<WikimediaPerson>> SearchByOccupationAsync(string occupation)
    {
        return await _context.WikimediaPeople
            .Where(x => x.Occupation != null && x.Occupation.Contains(occupation))
            .OrderBy(x => x.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<WikimediaPerson>> SearchByNationalityAsync(string nationality)
    {
        return await _context.WikimediaPeople
            .Where(x => x.Nationality == nationality)
            .OrderBy(x => x.Title)
            .ToListAsync();
    }
}