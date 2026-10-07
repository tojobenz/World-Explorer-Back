using AuthApi.Application.Common;
using AuthApi.Application.DTOs;

namespace AuthApi.Application.Interfaces;

/// <summary>
/// Interface service to manage places Wikimedia
/// </summary>
public interface IWikimediaPlaceService
{
    Task<WikimediaSearchResultDto> SearchPlacesAsync(WikimediaSearchRequestDto request);
    Task<WikimediaDetailDto> GetPlaceDetailsAsync(string pageId, Guid? userId = null);
    Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, string pageId);
    Task<WikimediaSearchResultDto> GetUserFavoritesAsync(Guid userId);
}