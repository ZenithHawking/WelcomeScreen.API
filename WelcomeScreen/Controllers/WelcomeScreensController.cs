using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WelcomeScreen.API.Data;
using WelcomeScreen.API.Models;
using WelcomeScreen.API.Services;
using Microsoft.EntityFrameworkCore;
using WelcomeScreen.API.Data;

namespace WelcomeScreen.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WelcomeScreensController(WelcomeScreenService service, AppDbContext db) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await service.GetAll(UserId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ws = await service.GetById(id, UserId);
        return ws == null ? NotFound() : Ok(ws);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWelcomeScreenDto dto)
    {
        var ws = await service.Create(UserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = ws.Id }, ws);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateWelcomeScreenDto dto)
        => await service.Update(id, UserId, dto) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => await service.Delete(id, UserId) ? NoContent() : NotFound();

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        await service.UpdateStatus(id, status);
        return NoContent();
    }
    [HttpPost("{id}/screenconfig")]
    public async Task<IActionResult> SaveScreenConfig(int id, [FromBody] SaveScreenConfigDto dto)
    {
        var ws = await db.WelcomeScreens
            .Include(w => w.ScreenConfig)
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == UserId);
        if (ws == null) return NotFound();

        if (ws.ScreenConfig == null)
        {
            ws.ScreenConfig = new ScreenConfig
            {
                WelcomeScreenId = id,
                BackgroundMediaId = dto.BackgroundMediaId,
                CanvasWidth = dto.CanvasWidth,
                CanvasHeight = dto.CanvasHeight,
                LayoutJson = dto.LayoutJson,
            };
            db.ScreenConfigs.Add(ws.ScreenConfig);
        }
        else
        {
            ws.ScreenConfig.BackgroundMediaId = dto.BackgroundMediaId;
            ws.ScreenConfig.CanvasWidth = dto.CanvasWidth;
            ws.ScreenConfig.CanvasHeight = dto.CanvasHeight;
            ws.ScreenConfig.LayoutJson = dto.LayoutJson;
            ws.ScreenConfig.UpdatedAt = DateTime.UtcNow;
        }
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id}/fields/{fieldId}")]
    public async Task<IActionResult> UpdateField(int id, int fieldId, [FromBody] UpdateFieldDto dto)
    {
        var field = await db.EventFields
            .FirstOrDefaultAsync(f => f.Id == fieldId && f.WelcomeScreenId == id);
        if (field == null) return NotFound();

        field.DisplayLabel = dto.DisplayLabel;
        field.Order = dto.Order;
        field.FontSize = dto.FontSize;
        field.FontFamily = dto.FontFamily;
        field.FontColor = dto.FontColor;
        field.FontWeight = dto.FontWeight;
        field.TextAlign = dto.TextAlign;
        field.PositionX = dto.PositionX;
        field.PositionY = dto.PositionY;
        field.Width = dto.Width;
        field.AnimationType = dto.AnimationType;
        field.AnimationDuration = dto.AnimationDuration;
        field.AnimationDelay = dto.AnimationDelay;

        await db.SaveChangesAsync();
        return Ok();
    }
    [HttpPost("{id}/fields/bulk")]
    public async Task<IActionResult> SaveFields(int id, [FromBody] List<SaveFieldDto> fields)
    {
        var ws = await db.WelcomeScreens.FirstOrDefaultAsync(w => w.Id == id && w.UserId == UserId);
        if (ws == null) return NotFound();

        // Xóa fields cũ
        var existing = db.EventFields.Where(f => f.WelcomeScreenId == id);
        db.EventFields.RemoveRange(existing);

        // Thêm fields mới
        for (int i = 0; i < fields.Count; i++)
        {
            db.EventFields.Add(new EventField
            {
                WelcomeScreenId = id,
                SourceColumn = fields[i].SourceColumn,
                DisplayLabel = fields[i].DisplayLabel,
                Order = i,
            });
        }
        await db.SaveChangesAsync();
        return Ok();
    }
    [HttpGet("{id}/processedguests")]
    public async Task<IActionResult> GetProcessedGuests(int id)
    {
        var ws = await db.WelcomeScreens
            .Include(w => w.DataSource)
            .Include(w => w.Fields)
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == UserId);

        if (ws?.DataSource == null) return NotFound();

        var ds = ws.DataSource;
        var fields = ws.Fields.ToList();
        if (fields.Count == 0 || ds.LastPolledId == 0) return Ok(new List<object>());

        try
        {
            var columns = fields.Select(f => f.SourceColumn).ToList();
            columns.Add(ds.TriggerColumn);
            columns = columns.Distinct().ToList();

            var colList = string.Join(", ", columns.Select(c => $"[{c}]"));
            var sql = $"SELECT {colList} FROM [{ds.TableName}] WHERE [{ds.TriggerColumn}] <= @lastId ORDER BY [{ds.TriggerColumn}]";

            await using var conn = new Microsoft.Data.SqlClient.SqlConnection(ds.ConnectionString);
            await conn.OpenAsync();
            var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@lastId", ds.LastPolledId);

            var rows = new List<Dictionary<string, object>>();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? "" : reader.GetValue(i).ToString()!;
                rows.Add(row);
            }
            return Ok(rows);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public record SaveFieldDto(string SourceColumn, string? DisplayLabel);
}
public record SaveScreenConfigDto(
    int? BackgroundMediaId,
    int CanvasWidth,
    int CanvasHeight,
    string? LayoutJson
);

public record UpdateFieldDto(
    string? DisplayLabel,
    int Order,
    int FontSize,
    string FontFamily,
    string FontColor,
    string FontWeight,
    string TextAlign,
    double PositionX,
    double PositionY,
    double Width,
    string AnimationType,
    int AnimationDuration,
    int AnimationDelay
);