using AuthApi.Application.DTOs;
using AuthApi.Application.Common;

namespace AuthApi.Application.Interfaces;

/// <summary>
/// Interface service to manage monuments Wikimedia
/// </summary>
public interface IWikimediaMonumentService
{
    Task<WikimediaSearchResultDto> SearchMonumentsAsync(WikimediaSearchRequestDto request);
    Task<WikimediaDetailDto> GetMonumentDetailsAsync(string pageId, Guid? userId = null);
    Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, string pageId);
    Task<WikimediaSearchResultDto> GetUserFavoritesAsync(Guid userId);
}