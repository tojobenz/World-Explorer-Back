using AuthApi.Application.Common;
using AuthApi.Application.DTOs;
using AuthApi.Application.Interfaces;
using AuthApi.Domain.Entities;
using AuthApi.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthApi.Application.Services;

/// <summary>
/// Service to manage personality Wikimedia
/// </summary>
public class WikimediaPersonService : IWikimediaPersonService
{
    private readonly IWikimediaService _wikimediaService;
    private readonly IWikimediaPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WikimediaPersonService> _logger;

    public WikimediaPersonService(
        IWikimediaService wikimediaService,
        IWikimediaPersonRepository personRepository,
        IUnitOfWork unitOfWork,
        ILogger<WikimediaPersonService> logger)
    {
        _wikimediaService = wikimediaService;
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<WikimediaSearchResultDto> SearchPeopleAsync(WikimediaSearchRequestDto request)
    {
        try
        {
            var result = await _wikimediaService.SearchPeopleAsync(
                request.Query, 
                request.Limit, 
                request.Language ?? "fr");

            if (result.Success && result.Data != null)
            {
                foreach (var item in result.Data)
                {
                    var existing = await _personRepository.GetByPageIdAsync(item.PageId);
                    if (existing == null)
                    {
                        var person = MapToEntity(item);
                        await _personRepository.AddAsync(person);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching people");
            return new WikimediaSearchResultDto
            {
                Success = false,
                Message = "Error searching people",
                Data = null
            };
        }
    }

    public async Task<WikimediaDetailDto> GetPersonDetailsAsync(string pageId, Guid? userId = null)
    {
        try
        {
            var result = await _wikimediaService.GetDetailsAsync(pageId);
            
            if (result.Success && result.Data != null)
            {
                var existing = await _personRepository.GetByPageIdAsync(pageId);
                if (existing != null)
                {
                    existing.Extract = result.Data.Extract;
                    existing.ThumbnailUrl = result.Data.ThumbnailUrl;
                    existing.FullUrl = result.Data.FullUrl;
                    await _personRepository.UpdateAsync(existing);
                }
                else
                {
                    var person = MapToEntity(result.Data);
                    await _personRepository.AddAsync(person);
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
            _logger.LogError(ex, "Error getting person details for {PageId}", pageId);
            return new WikimediaDetailDto
            {
                Success = false,
                Message = "Error retrieving person details",
                Data = null
            };
        }
    }

    public async Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, string pageId)
    {
        try
        {
            var person = await _personRepository.ToggleFavoriteAsync(userId, pageId);
            
            if (person == null)
            {
                var details = await _wikimediaService.GetDetailsAsync(pageId);
                if (details.Success && details.Data != null)
                {
                    person = MapToEntity(details.Data);
                    person.UserId = userId;
                    person.IsUserFavorite = true;
                    await _personRepository.AddAsync(person);
                    await _unitOfWork.SaveChangesAsync();
                    return ApiResponse<bool>.SuccessResponse(true, "Favorite toggled successfully");
                }

                return ApiResponse<bool>.ErrorResponse("Person not found");
            }

            return ApiResponse<bool>.SuccessResponse(person.IsUserFavorite, "Favorite toggled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for person {PageId}", pageId);
            return ApiResponse<bool>.ErrorResponse("Error toggling favorite");
        }
    }

    public async Task<WikimediaSearchResultDto> GetUserFavoritesAsync(Guid userId)
    {
        try
        {
            var favorites = await _personRepository.GetUserFavoritesAsync(userId);
            
            var items = favorites.Select(f => new WikimediaItemDto
            {
                PageId = f.PageId,
                Title = f.Title,
                Description = f.Description,
                Extract = f.Extract,
                ThumbnailUrl = f.ThumbnailUrl,
                FullUrl = f.FullUrl,
                Category = "person",
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

    private static WikimediaPerson MapToEntity(WikimediaItemDto dto)
    {
        return new WikimediaPerson
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