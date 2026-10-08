using System.Security.Claims;
using AuthApi.Application.DTOs;
using AuthApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WikimediaMonumentController : ControllerBase
{
    private readonly IWikimediaMonumentService _wikimediaMonumentService;
    private readonly ILogger<WikimediaMonumentController> _logger;

    public WikimediaMonumentController(
        IWikimediaMonumentService wikimediaMonumentService,
        ILogger<WikimediaMonumentController> logger)
    {
        _wikimediaMonumentService = wikimediaMonumentService;
        _logger = logger;
    }

    /// <summary>
    /// search monument
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WikimediaSearchRequestDto request)
    {
        try
        {
            var result = await _wikimediaMonumentService.SearchMonumentsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during monument search");
            return StatusCode(500, new { message = "An error occurred during search" });
        }
    }

    /// <summary>
    /// Get detail monument
    /// </summary>
    [HttpGet("{pageId}")]
    public async Task<IActionResult> GetDetails(string pageId)
    {
        try
        {
            var userId = User?.FindFirst("sub")?.Value ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _wikimediaMonumentService.GetMonumentDetailsAsync(
                pageId,
                userId != null ? Guid.Parse(userId) : null);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting monument details for {PageId}", pageId);
            return StatusCode(500, new { message = "An error occurred retrieving details" });
        }
    }

    /// <summary>
    /// Add/remove monument from favoris
    ///
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
            var result = await _wikimediaMonumentService.ToggleFavoriteAsync(userId, pageId);
            
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for monument {PageId}", pageId);
            return StatusCode(500, new { message = "An error occurred toggling favorite" });
        }
    }

    /// <summary>
    /// Get favoris user
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
            var result = await _wikimediaMonumentService.GetUserFavoritesAsync(userId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user favorites");
            return StatusCode(500, new { message = "An error occurred retrieving favorites" });
        }
    }
}