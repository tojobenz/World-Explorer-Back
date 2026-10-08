using System.Security.Claims;
using AuthApi.Application.DTOs;
using AuthApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WikimediaFactController : ControllerBase
{
    private readonly IWikimediaFactService _wikimediaFactService;
    private readonly ILogger<WikimediaFactController> _logger;

    public WikimediaFactController(
        IWikimediaFactService wikimediaFactService,
        ILogger<WikimediaFactController> logger)
    {
        _wikimediaFactService = wikimediaFactService;
        _logger = logger;
    }

    /// <summary>
    /// search historic fact in wikimedia
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WikimediaSearchRequestDto request)
    {
        try
        {
            var result = await _wikimediaFactService.SearchFactsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during fact search");
            return StatusCode(500, new { message = "An error occurred during search" });
        }
    }

    /// <summary>
    /// Get The historic detail fact
    /// </summary>
    [HttpGet("{pageId}")]
    public async Task<IActionResult> GetDetails(string pageId)
    {
        try
        {
            var userId = User?.FindFirst("sub")?.Value ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _wikimediaFactService.GetFactDetailsAsync(
                pageId, 
                userId != null ? Guid.Parse(userId) : null);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting fact details for {PageId}", pageId);
            return StatusCode(500, new { message = "An error occurred retrieving details" });
        }
    }

    /// <summary>
    /// add/remove fact from favorite
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
            var result = await _wikimediaFactService.ToggleFavoriteAsync(userId, pageId);
            
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for fact {PageId}", pageId);
            return StatusCode(500, new { message = "An error occurred toggling favorite" });
        }
    }

    /// <summary>
    /// Get Favorite user
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
            var result = await _wikimediaFactService.GetUserFavoritesAsync(userId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user favorites");
            return StatusCode(500, new { message = "An error occurred retrieving favorites" });
        }
    }
}