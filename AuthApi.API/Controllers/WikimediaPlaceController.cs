using System.Security.Claims;
using AuthApi.Application.DTOs;
using AuthApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WikimediaPlaceController : ControllerBase
{
    private readonly IWikimediaPlaceService _wikimediaPlaceService;
    private readonly ILogger<WikimediaPlaceController> _logger;

    public WikimediaPlaceController(
        IWikimediaPlaceService wikimediaPlaceService,
        ILogger<WikimediaPlaceController> logger)
    {
        _wikimediaPlaceService = wikimediaPlaceService;
        _logger = logger;
    }

    /// <summary>
    /// search place
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WikimediaSearchRequestDto request)
    {
        try
        {
            var result = await _wikimediaPlaceService.SearchPlacesAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during place search");
            return StatusCode(500, new { message = "An error occurred during search" });
        }
    }

    /// <summary>
    /// Get detail place
    /// </summary>
    [HttpGet("{pageId}")]
    public async Task<IActionResult> GetDetails(string pageId)
    {
        try
        {
            var userId = User?.FindFirst("sub")?.Value ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _wikimediaPlaceService.GetPlaceDetailsAsync(
                pageId, 
                userId != null ? Guid.Parse(userId) : null);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting place details for {PageId}", pageId);
            return StatusCode(500, new { message = "An error occurred retrieving details" });
        }
    }

    /// <summary>
    /// Add/Remove place favoir
    /// </summary>
    [HttpPost("{pageId}/toggle-favorite")]
    public async Task<IActionResult> ToggleFavorite(string pageId)
    {
        try
        {
            var userIdClaim = User?.FindFirst("sub")?.Value ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var userId = Guid.Parse(userIdClaim);
            var result = await _wikimediaPlaceService.ToggleFavoriteAsync(userId, pageId);
            
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for place {PageId}", pageId);
            return StatusCode(500, new { message = "An error occurred toggling favorite" });
        }
    }

    /// <summary>
    /// Get favori user
    /// </summary>
    [HttpGet("favorites")]
    public async Task<IActionResult> GetUserFavorites()
    {
        try
        {
            var userIdClaim = User?.FindFirst("sub")?.Value ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var userId = Guid.Parse(userIdClaim);
            var result = await _wikimediaPlaceService.GetUserFavoritesAsync(userId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user favorites");
            return StatusCode(500, new { message = "An error occurred retrieving favorites" });
        }
    }
}