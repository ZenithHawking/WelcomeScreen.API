using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WelcomeScreen.API.Services;

namespace WelcomeScreen.API.Controllers;

[ApiController]
[Route("api/media")]
[Authorize]
public class MediaController(MediaService mediaService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? welcomeScreenId)
        => Ok(await mediaService.GetAll(welcomeScreenId));

    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] int? welcomeScreenId)
    {
        var allowed = new[] { "image/png", "image/jpeg", "image/webp", "video/mp4" };
        if (!allowed.Contains(file.ContentType))
            return BadRequest(new { message = "Chỉ chấp nhận PNG, JPG, WebP, MP4" });

        if (file.Length > 500 * 1024 * 1024) // 500MB
            return BadRequest(new { message = "File quá lớn (tối đa 500MB)" });

        var media = await mediaService.Upload(file, welcomeScreenId);
        return Ok(media);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => await mediaService.Delete(id) ? NoContent() : NotFound();
}