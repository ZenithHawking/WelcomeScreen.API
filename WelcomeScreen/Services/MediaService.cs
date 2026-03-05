using Microsoft.EntityFrameworkCore;
using WelcomeScreen.API.Data;
using WelcomeScreen.API.Models;

namespace WelcomeScreen.API.Services;

public class MediaService(AppDbContext db, IConfiguration config, IWebHostEnvironment env)
{
    private string UploadPath => Path.Combine(env.WebRootPath ?? "wwwroot", config["FileStorage:UploadPath"] ?? "uploads");

    public async Task<MediaFile> Upload(IFormFile file, int? welcomeScreenId)
    {
        Directory.CreateDirectory(UploadPath);
        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(UploadPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var media = new MediaFile
        {
            WelcomeScreenId = welcomeScreenId,
            FileName = fileName,
            OriginalName = file.FileName,
            FilePath = $"/uploads/{fileName}",
            FileType = file.ContentType,
            FileSize = file.Length
        };
        db.MediaFiles.Add(media);
        await db.SaveChangesAsync();
        return media;
    }

    public async Task<List<MediaFile>> GetAll(int? welcomeScreenId = null)
        => await db.MediaFiles
            .Where(m => welcomeScreenId == null || m.WelcomeScreenId == welcomeScreenId || m.WelcomeScreenId == null)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<bool> Delete(int id)
    {
        var media = await db.MediaFiles.FindAsync(id);
        if (media == null) return false;
        var fullPath = Path.Combine(env.WebRootPath ?? "wwwroot", media.FilePath.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);
        db.MediaFiles.Remove(media);
        await db.SaveChangesAsync();
        return true;
    }
}