using AuthApi.Application.Common;
using AuthApi.Application.DTOs;
using AuthApi.Application.Interfaces;
using AuthApi.Domain.Entities;
using AuthApi.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthApi.Application.Services;

/// <summary>
/// Service to manage monuments Wikimedia
/// </summary>
public class WikimediaMonumentService : IWikimediaMonumentService
{
    private readonly IWikimediaService _wikimediaService;
    private readonly IWikimediaMonumentRepository _monumentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WikimediaMonumentService> _logger;

    public WikimediaMonumentService(
        IWikimediaService wikimediaService,
        IWikimediaMonumentRepository monumentRepository,
        IUnitOfWork unitOfWork,
        ILogger<WikimediaMonumentService> logger)
    {
        _wikimediaService = wikimediaService;
        _monumentRepository = monumentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<WikimediaSearchResultDto> SearchMonumentsAsync(WikimediaSearchRequestDto request)
    {
        try
        {
            var result = await _wikimediaService.SearchMonumentsAsync(
                request.Query, 
                request.Limit, 
                request.Language ?? "fr");

            if (result.Success && result.Data != null)
            {
                foreach (var item in result.Data)
                {
                    var existing = await _monumentRepository.GetByPageIdAsync(item.PageId);
                    if (existing == null)
                    {
                        var monument = MapToEntity(item);
                        await _monumentRepository.AddAsync(monument);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching monuments");
            return new WikimediaSearchResultDto
            {
                Success = false,
                Message = "Error searching monuments",
                Data = null
            };
        }
    }

    public async Task<WikimediaDetailDto> GetMonumentDetailsAsync(string pageId, Guid? userId = null)
    {
        try
        {
            var result = await _wikimediaService.GetDetailsAsync(pageId);
            
            if (result.Success && result.Data != null)
            {
                var existing = await _monumentRepository.GetByPageIdAsync(pageId);
                if (existing != null)
                {
                    existing.Extract = result.Data.Extract;
                    existing.ThumbnailUrl = result.Data.ThumbnailUrl;
                    existing.FullUrl = result.Data.FullUrl;
                    if (result.Data.Coordinates != null && result.Data.Coordinates.Count >= 2)
                    {
                        if (double.TryParse(result.Data.Coordinates[0], out var lat))
                            existing.Latitude = lat;
                        if (double.TryParse(result.Data.Coordinates[1], out var lon))
                            existing.Longitude = lon;
                    }
                    await _monumentRepository.UpdateAsync(existing);
                }
                else
                {
                    var monument = MapToEntity(result.Data);
                    await _monumentRepository.AddAsync(monument);
                }
                await _unitOfWork.SaveChangesAsync();

                if (userId.HasValue && existing != null)
                {
                    result.Data.IsFavorite = existing.IsUserFavorite && existing.UserId == userId;
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting monument details for {PageId}", pageId);
            return new WikimediaDetailDto
            {
                Success = false,
                Message = "Error retrieving monument details",
                Data = null
            };
        }
    }

    public async Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, string pageId)
    {
        try
        {
            var monument = await _monumentRepository.ToggleFavoriteAsync(userId, pageId);
            
            if (monument == null)
            {
                var details = await _wikimediaService.GetDetailsAsync(pageId);
                if (details.Success && details.Data != null)
                {
                    monument = MapToEntity(details.Data);
                    monument.UserId = userId;
                    monument.IsUserFavorite = true;
                    await _monumentRepository.AddAsync(monument);
                    await _unitOfWork.SaveChangesAsync();
                    return ApiResponse<bool>.SuccessResponse(true, "Favorite toggled successfully");
                }

                return ApiResponse<bool>.ErrorResponse("Monument not found");
            }

            return ApiResponse<bool>.SuccessResponse(monument.IsUserFavorite, "Favorite toggled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for monument {PageId}", pageId);
            return ApiResponse<bool>.ErrorResponse("Error toggling favorite");
        }
    }

    public async Task<WikimediaSearchResultDto> GetUserFavoritesAsync(Guid userId)
    {
        try
        {
            var favorites = await _monumentRepository.GetUserFavoritesAsync(userId);
            
            var items = favorites.Select(f => new WikimediaItemDto
            {
                PageId = f.PageId,
                Title = f.Title,
                Description = f.Description,
                Extract = f.Extract,
                ThumbnailUrl = f.ThumbnailUrl,
                FullUrl = f.FullUrl,
                Category = "monument",
                Coordinates = f.Latitude.HasValue && f.Longitude.HasValue
                    ? new List<string> { f.Latitude.Value.ToString(), f.Longitude.Value.ToString() }
                    : null,
                IsFavorite = true
            }).ToList();

            return new WikimediaSearchResultDto
            {
                Success = true,
                Message = $"{items.Count} favorites found",
                Data = items
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user favorites");
            return new WikimediaSearchResultDto
            {
                Success = false,
                Message = "Error retrieving favorites",
                Data = null
            };
        }
    }

    private static WikimediaMonument MapToEntity(WikimediaItemDto dto)
    {
        return new WikimediaMonument
        {
            PageId = dto.PageId,
            Title = dto.Title,
            Description = dto.Description,
            Extract = dto.Extract,
            ThumbnailUrl = dto.ThumbnailUrl,
            FullUrl = dto.FullUrl,
            Latitude = dto.Coordinates != null && dto.Coordinates.Count >= 2 && double.TryParse(dto.Coordinates[0], out var lat)
                ? lat
                : null,
            Longitude = dto.Coordinates != null && dto.Coordinates.Count >= 2 && double.TryParse(dto.Coordinates[1], out var lon)
                ? lon
                : null
        };
    }
}