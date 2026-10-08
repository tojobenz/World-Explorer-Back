using AuthApi.Application.Common;
using AuthApi.Application.DTOs;
using AuthApi.Application.Interfaces;
using AuthApi.Domain.Entities;
using AuthApi.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthApi.Application.Services;

/// <summary>
/// Service applicatif pour la gestion des faits Wikimedia
/// </summary>
public class WikimediaFactService : IWikimediaFactService
{
    private readonly IWikimediaService _wikimediaService;
    private readonly IWikimediaFactRepository _factRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WikimediaFactService> _logger;

    public WikimediaFactService(
        IWikimediaService wikimediaService,
        IWikimediaFactRepository factRepository,
        IUnitOfWork unitOfWork,
        ILogger<WikimediaFactService> logger)
    {
        _wikimediaService = wikimediaService;
        _factRepository = factRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<WikimediaSearchResultDto> SearchFactsAsync(WikimediaSearchRequestDto request)
    {
        try
        {
            var result = await _wikimediaService.SearchFactsAsync(
                request.Query, 
                request.Limit, 
                request.Language ?? "fr");

            if (result.Success && result.Data != null)
            {
                try
                {
                    foreach (var item in result.Data)
                    {
                        var existing = await _factRepository.GetByPageIdAsync(item.PageId);
                        if (existing == null)
                        {
                            var fact = MapToEntity(item);
                            await _factRepository.AddAsync(fact);
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception dbEx)
                {
                    _logger.LogWarning(dbEx, "Failed to cache search results to database");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching facts");
            return new WikimediaSearchResultDto
            {
                Success = false,
                Message = "Error searching facts",
                Data = null
            };
        }
    }

    public async Task<WikimediaDetailDto> GetFactDetailsAsync(string pageId, Guid? userId = null)
    {
        try
        {
            var result = await _wikimediaService.GetDetailsAsync(pageId);
            
            if (result.Success && result.Data != null)
            {
                var existing = await _factRepository.GetByPageIdAsync(pageId);
                if (existing != null)
                {
                    existing.Extract = result.Data.Extract;
                    existing.ThumbnailUrl = result.Data.ThumbnailUrl;
                    existing.FullUrl = result.Data.FullUrl;
                    await _factRepository.UpdateAsync(existing);
                }
                else
                {
                    var fact = MapToEntity(result.Data);
                    await _factRepository.AddAsync(fact);
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
            _logger.LogError(ex, "Error getting fact details for {PageId}", pageId);
            return new WikimediaDetailDto
            {
                Success = false,
                Message = "Error retrieving fact details",
                Data = null
            };
        }
    }

    public async Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, string pageId)
    {
        try
        {
            var fact = await _factRepository.ToggleFavoriteAsync(userId, pageId);
            
            if (fact == null)
            {
                var details = await _wikimediaService.GetDetailsAsync(pageId);
                if (details.Success && details.Data != null)
                {
                    fact = MapToEntity(details.Data);
                    fact.UserId = userId;
                    fact.IsUserFavorite = true;
                    await _factRepository.AddAsync(fact);
                    await _unitOfWork.SaveChangesAsync();
                    return ApiResponse<bool>.SuccessResponse(true, "Favorite toggled successfully");
                }

                return ApiResponse<bool>.ErrorResponse("Fact not found");
            }

            return ApiResponse<bool>.SuccessResponse(fact.IsUserFavorite, "Favorite toggled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for fact {PageId}", pageId);
            return ApiResponse<bool>.ErrorResponse("Error toggling favorite");
        }
    }

    public async Task<WikimediaSearchResultDto> GetUserFavoritesAsync(Guid userId)
    {
        try
        {
            var favorites = await _factRepository.GetUserFavoritesAsync(userId);
            
            var items = favorites.Select(f => new WikimediaItemDto
            {
                PageId = f.PageId,
                Title = f.Title,
                Description = f.Description,
                Extract = f.Extract,
                ThumbnailUrl = f.ThumbnailUrl,
                FullUrl = f.FullUrl,
                Category = "fact",
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

    private static WikimediaFact MapToEntity(WikimediaItemDto dto)
    {
        return new WikimediaFact
        {
            PageId = dto.PageId,
            Title = dto.Title,
            Description = dto.Description,
            Extract = dto.Extract,
            ThumbnailUrl = dto.ThumbnailUrl,
            FullUrl = dto.FullUrl
        };
    }
}