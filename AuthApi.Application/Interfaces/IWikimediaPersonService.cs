using AuthApi.Application.DTOs;
using AuthApi.Application.Common;

namespace AuthApi.Application.Interfaces;

/// <summary>
/// Interface service to manage personnality Wikimedia
/// </summary>
public interface IWikimediaPersonService
{
    Task<WikimediaSearchResultDto> SearchPeopleAsync(WikimediaSearchRequestDto request);
    Task<WikimediaDetailDto> GetPersonDetailsAsync(string pageId, Guid? userId = null);
    Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, string pageId);
    Task<WikimediaSearchResultDto> GetUserFavoritesAsync(Guid userId);
}