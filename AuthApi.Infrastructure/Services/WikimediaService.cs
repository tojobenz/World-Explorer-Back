using System.Text.Json;
using AuthApi.Application.DTOs;
using AuthApi.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthApi.Infrastructure.Services;

/// <summary>
/// Service to communicate to the API Wikimedia
/// </summary>
public class WikimediaService : IWikimediaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WikimediaService> _logger;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public WikimediaService(
        HttpClient httpClient,
        ILogger<WikimediaService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WorldExplorerApp/1.0 (contact@worldexplorer.com)");
        }
    }

    public async Task<WikimediaSearchResultDto> SearchAsync(WikimediaSearchRequestDto request)
    {
        try
        {
            var language = request.Language ?? "fr";
            var baseUrl = $"https://{language}.wikipedia.org/w/api.php";
            
            var queryParams = new Dictionary<string, string>
            {
                { "action", "query" },
                { "list", "search" },
                { "srsearch", request.Query },
                { "limit", request.Limit.ToString() },
                { "format", "json" },
                { "origin", "*" }
            };

            var url = $"{baseUrl}?{await new FormUrlEncodedContent(queryParams).ReadAsStringAsync()}";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WikimediaApiResponse>(jsonContent, _jsonOptions);

            if (result?.Query?.Search == null)
            {
                return new WikimediaSearchResultDto
                {
                    Success = false,
                    Message = "No results found",
                    Data = new List<WikimediaItemDto>()
                };
            }

            var items = result.Query.Search.Select(s => new WikimediaItemDto
            {
                PageId = s.PageId.ToString(),
                Title = s.Title,
                Description = s.Snippet?.Replace("<span class=\"searchmatch\">", "").Replace("</span>", ""),
                Category = request.Category
            }).ToList();

            return new WikimediaSearchResultDto
            {
                Success = true,
                Message = $"{items.Count} results found",
                Data = items
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching Wikimedia");
            return new WikimediaSearchResultDto
            {
                Success = false,
                Message = $"Error searching Wikimedia: {ex.Message}",
                Data = null
            };
        }
    }

    public async Task<WikimediaDetailDto> GetDetailsAsync(string pageId, string language = "fr")
    {
        try
        {
            var baseUrl = $"https://{language}.wikipedia.org/w/api.php";
            
            var queryParams = new Dictionary<string, string>
            {
                { "action", "query" },
                { "prop", "extracts|pageimages|info|coordinates" },
                { "pageids", pageId },
                { "exintro", "1" },
                { "explaintext", "1" },
                { "pithumbsize", "500" },
                { "inprop", "url" },
                { "format", "json" },
                { "origin", "*" }
            };

            var url = $"{baseUrl}?{await new FormUrlEncodedContent(queryParams).ReadAsStringAsync()}";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WikimediaDetailApiResponse>(jsonContent, _jsonOptions);

            var page = result?.Query?.Pages?.Values.FirstOrDefault();
            if (page == null)
            {
                return new WikimediaDetailDto
                {
                    Success = false,
                    Message = "Page not found",
                    Data = null
                };
            }

            var item = new WikimediaItemDto
            {
                PageId = pageId,
                Title = page.Title,
                Extract = page.Extract,
                ThumbnailUrl = page.Thumbnail?.Source,
                FullUrl = page.FullUrl,
                Coordinates = page.Coordinates != null && page.Coordinates.Any()
                ? new List<string> { page.Coordinates.First().Lat.ToString(), page.Coordinates.First().Lon.ToString() }
                : null
            };

            return new WikimediaDetailDto
            {
                Success = true,
                Message = "Details retrieved successfully",
                Data = item
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Wikimedia details for page {PageId}", pageId);
            return new WikimediaDetailDto
            {
                Success = false,
                Message = $"Error retrieving details: {ex.Message}",
                Data = null
            };
        }
    }

    public async Task<WikimediaSearchResultDto> SearchPlacesAsync(string query, int limit = 10, string language = "fr")
    {
        return await SearchAsync(new WikimediaSearchRequestDto
        {
            Query = $"{query} lieu city",
            Category = "place",
            Limit = limit,
            Language = language
        });
    }

    public async Task<WikimediaSearchResultDto> SearchPeopleAsync(string query, int limit = 10, string language = "fr")
    {
        return await SearchAsync(new WikimediaSearchRequestDto
        {
            Query = $"{query} person",
            Category = "person",
            Limit = limit,
            Language = language
        });
    }

    public async Task<WikimediaSearchResultDto> SearchMonumentsAsync(string query, int limit = 10, string language = "fr")
    {
        return await SearchAsync(new WikimediaSearchRequestDto
        {
            Query = $"{query} monument",
            Category = "monument",
            Limit = limit,
            Language = language
        });
    }

    public async Task<WikimediaSearchResultDto> SearchFactsAsync(string query, int limit = 10, string language = "fr")
    {
        return await SearchAsync(new WikimediaSearchRequestDto
        {
            Query = $"{query} history",
            Category = "fact",
            Limit = limit,
            Language = language
        });
    }

    // Internal DTOs for JSON deserialization
    private class WikimediaApiResponse
    {
        public WikimediaQuery? Query { get; set; }
    }

    private class WikimediaQuery
    {
        public List<WikimediaSearchItem>? Search { get; set; }
    }

    private class WikimediaSearchItem
    {
        public int PageId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Snippet { get; set; }
    }

    private class WikimediaDetailApiResponse
    {
        public WikimediaDetailQuery? Query { get; set; }
    }

    private class WikimediaDetailQuery
    {
        public Dictionary<string, WikimediaPage>? Pages { get; set; }
    }

    private class WikimediaPage
    {
        public string Title { get; set; } = string.Empty;
        public string? Extract { get; set; }
        public WikimediaThumbnail? Thumbnail { get; set; }
        public string? FullUrl { get; set; }
        public List<WikimediaCoordinate>? Coordinates { get; set; }
    }

    private class WikimediaThumbnail
    {
        public string? Source { get; set; }
    }

    private class WikimediaCoordinate
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}