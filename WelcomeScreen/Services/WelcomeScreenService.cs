using Microsoft.EntityFrameworkCore;
using WelcomeScreen.API.Data;
using WelcomeScreen.API.Models;

namespace WelcomeScreen.API.Services;

public class WelcomeScreenService(AppDbContext db)
{
    public async Task<List<WelcomeScreen.API.Models.WelcomeScreen>> GetAll(int userId)
        => await db.WelcomeScreens
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();

    public async Task<WelcomeScreen.API.Models.WelcomeScreen?> GetById(int id, int userId)
        => await db.WelcomeScreens
            .Include(w => w.DataSource)
            .Include(w => w.Fields.OrderBy(f => f.Order))
            .Include(w => w.ScreenConfig)
                .ThenInclude(s => s.BackgroundMedia)
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

    public async Task<WelcomeScreen.API.Models.WelcomeScreen> Create(int userId, CreateWelcomeScreenDto dto)
    {
        var ws = new WelcomeScreen.API.Models.WelcomeScreen
        {
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
            EventDate = dto.EventDate
        };
        db.WelcomeScreens.Add(ws);
        await db.SaveChangesAsync();
        return ws;
    }

    public async Task<bool> Update(int id, int userId, CreateWelcomeScreenDto dto)
    {
        var ws = await db.WelcomeScreens.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (ws == null) return false;
        ws.Name = dto.Name;
        ws.Description = dto.Description;
        ws.EventDate = dto.EventDate;
        ws.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id, int userId)
    {
        var ws = await db.WelcomeScreens.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (ws == null) return false;
        db.WelcomeScreens.Remove(ws);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task UpdateStatus(int id, string status)
    {
        var ws = await db.WelcomeScreens.FindAsync(id);
        if (ws == null) return;
        ws.Status = status;
        ws.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }
}

// DTOs
public record CreateWelcomeScreenDto(string Name, string? Description, DateTime EventDate);