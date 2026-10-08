using AuthApi.Application.DTOs;
using AuthApi.Application.Common;

namespace AuthApi.Application.Interfaces;

/// <summary>
/// Interface service to manage fact wikimedia
/// </summary>
public interface IWikimediaFactService
{
    Task<WikimediaSearchResultDto> SearchFactsAsync(WikimediaSearchRequestDto request);
    Task<WikimediaDetailDto> GetFactDetailsAsync(string pageId, Guid? userId = null);
    Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, string pageId);
    Task<WikimediaSearchResultDto> GetUserFavoritesAsync(Guid userId);
}