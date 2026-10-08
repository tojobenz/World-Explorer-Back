using System.Security.Claims;
using AuthApi.Application.DTOs;
using AuthApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WikimediaPersonController : ControllerBase
{
    private readonly IWikimediaPersonService _wikimediaPersonService;
    private readonly ILogger<WikimediaPersonController> _logger;

    public WikimediaPersonController(
        IWikimediaPersonService wikimediaPersonService,
        ILogger<WikimediaPersonController> logger)
    {
        _wikimediaPersonService = wikimediaPersonService;
        _logger = logger;
    }

    /// <summary>
    /// Rechercher des personnalités sur Wikimedia
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WikimediaSearchRequestDto request)
    {
        try
        {
            var result = await _wikimediaPersonService.SearchPeopleAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during person search");
            return StatusCode(500, new { message = "An error occurred during search" });
        }
    }

    /// <summary>
    /// Obtenir les détails d'une personnalité
    /// </summary>
    [HttpGet("{pageId}")]
    public async Task<IActionResult> GetDetails(string pageId)
    {
        try
        {
            var userId = User?.FindFirst("sub")?.Value ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _wikimediaPersonService.GetPersonDetailsAsync(
                pageId, 
                userId != null ? Guid.Parse(userId) : null);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting person details for {PageId}", pageId);
            return StatusCode(500, new { message = "An error occurred retrieving details" });
        }
    }

    /// <summary>
    /// Ajouter/Retirer une personnalité des favoris
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
            var result = await _wikimediaPersonService.ToggleFavoriteAsync(userId, pageId);
            
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for person {PageId}", pageId);
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
            var result = await _wikimediaPersonService.GetUserFavoritesAsync(userId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user favorites");
            return StatusCode(500, new { message = "An error occurred retrieving favorites" });
        }
    }
}