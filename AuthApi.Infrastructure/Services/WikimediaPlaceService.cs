using AuthApi.Application.Common;
using AuthApi.Application.DTOs;
using AuthApi.Application.Interfaces;
using AuthApi.Domain.Entities;
using AuthApi.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthApi.Application.Services;

/// <summary>
/// Service to manage places Wikimedia
/// </summary>
public class WikimediaPlaceService : IWikimediaPlaceService
{
    private readonly IWikimediaService _wikimediaService;
    private readonly IWikimediaPlaceRepository _placeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WikimediaPlaceService> _logger;

    public WikimediaPlaceService(
        IWikimediaService wikimediaService,
        IWikimediaPlaceRepository placeRepository,
        IUnitOfWork unitOfWork,
        ILogger<WikimediaPlaceService> logger)
    {
        _wikimediaService = wikimediaService;
        _placeRepository = placeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<WikimediaSearchResultDto> SearchPlacesAsync(WikimediaSearchRequestDto request)
    {
        try
        {
            var result = await _wikimediaService.SearchPlacesAsync(
                request.Query, 
                request.Limit, 
                request.Language ?? "fr");

            if (result.Success && result.Data != null)
            {
                // Save to database if not exists
                foreach (var item in result.Data)
                {
                    var existing = await _placeRepository.GetByPageIdAsync(item.PageId);
                    if (existing == null)
                    {
                        var place = MapToEntity(item);
                        await _placeRepository.AddAsync(place);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching places");
            return new WikimediaSearchResultDto
            {
                Success = false,
                Message = "Error searching places",
                Data = null
            };
        }
    }

    public async Task<WikimediaDetailDto> GetPlaceDetailsAsync(string pageId, Guid? userId = null)
    {
        try
        {
            var result = await _wikimediaService.GetDetailsAsync(pageId);
            
            if (result.Success && result.Data != null)
            {
                // Update or create in database
                var existing = await _placeRepository.GetByPageIdAsync(pageId);
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
                    await _placeRepository.UpdateAsync(existing);
                }
                else
                {
                    var place = MapToEntity(result.Data);
                    await _placeRepository.AddAsync(place);
                }
                await _unitOfWork.SaveChangesAsync();

                // Check if favorite
                if (userId.HasValue && existing != null)
                {
                    result.Data.IsFavorite = existing.IsUserFavorite && existing.UserId == userId;
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting place details for {PageId}", pageId);
            return new WikimediaDetailDto
            {
                Success = false,
                Message = "Error retrieving place details",
                Data = null
            };
        }
    }

    public async Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, string pageId)
    {
        try
        {
            var place = await _placeRepository.ToggleFavoriteAsync(userId, pageId);
            
            if (place == null)
            {
                return ApiResponse<bool>.ErrorResponse("Place not found");
            }

            return ApiResponse<bool>.SuccessResponse(place.IsUserFavorite, "Favorite toggled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for place {PageId}", pageId);
            return ApiResponse<bool>.ErrorResponse("Error toggling favorite");
        }
    }

    public async Task<WikimediaSearchResultDto> GetUserFavoritesAsync(Guid userId)
    {
        try
        {
            var favorites = await _placeRepository.GetUserFavoritesAsync(userId);
            
            var items = favorites.Select(f => new WikimediaItemDto
            {
                PageId = f.PageId,
                Title = f.Title,
                Description = f.Description,
                Extract = f.Extract,
                ThumbnailUrl = f.ThumbnailUrl,
                FullUrl = f.FullUrl,
                Category = "place",
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

    private static WikimediaPlace MapToEntity(WikimediaItemDto dto)
    {
        return new WikimediaPlace
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