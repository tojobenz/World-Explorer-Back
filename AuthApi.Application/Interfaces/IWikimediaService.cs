using AuthApi.Application.DTOs;

namespace AuthApi.Application.Interfaces;

/// <summary>
/// Interface service Wikimedia (external API)
/// </summary>
public interface IWikimediaService
{
    Task<WikimediaSearchResultDto> SearchAsync(WikimediaSearchRequestDto request);
    Task<WikimediaDetailDto> GetDetailsAsync(string pageId, string language = "fr");
    Task<WikimediaSearchResultDto> SearchPlacesAsync(string query, int limit = 10, string language = "fr");
    Task<WikimediaSearchResultDto> SearchPeopleAsync(string query, int limit = 10, string language = "fr");
    Task<WikimediaSearchResultDto> SearchMonumentsAsync(string query, int limit = 10, string language = "fr");
    Task<WikimediaSearchResultDto> SearchFactsAsync(string query, int limit = 10, string language = "fr");
}