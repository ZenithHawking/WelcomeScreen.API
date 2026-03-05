using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WelcomeScreen.API.Data;
using WelcomeScreen.API.Hubs;
using Microsoft.Data.SqlClient;

namespace WelcomeScreen.API.Services;

public class PollingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<DisplayHub> _hubContext;
    private readonly ILogger<PollingService> _logger;

    public PollingService(
        IServiceScopeFactory scopeFactory,
        IHubContext<DisplayHub> hubContext,
        ILogger<PollingService> logger)
    {
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Polling Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PollAllActiveScreens();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Polling error");
            }

            await Task.Delay(2000, stoppingToken);
        }
    }

    private async Task PollAllActiveScreens()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var activeScreens = await db.WelcomeScreens
            .Include(w => w.DataSource)
            .Include(w => w.Fields.OrderBy(f => f.Order))
            .Where(w => w.Status == "Active" && w.DataSource != null)
            .ToListAsync();

        foreach (var screen in activeScreens)
        {
            await PollScreen(screen, db);
        }
    }

    private async Task PollScreen(
        WelcomeScreen.API.Models.WelcomeScreen screen,
        AppDbContext db)
    {
        var ds = screen.DataSource!;
        var fields = screen.Fields.ToList();
        if (fields.Count == 0) return;

        try
        {
            var columns = fields.Select(f => f.SourceColumn).ToList();
            columns.Add(ds.TriggerColumn);
            columns = columns.Distinct().ToList();

            var colList = string.Join(", ", columns.Select(c => $"[{c}]"));
            var sql = $"SELECT {colList} FROM [{ds.TableName}] WHERE [{ds.TriggerColumn}] > @lastId ORDER BY [{ds.TriggerColumn}]";

            var newGuests = new List<Dictionary<string, object>>();

            await using var conn = new SqlConnection(ds.ConnectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@lastId", ds.LastPolledId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? "" : reader.GetValue(i).ToString()!;
                newGuests.Add(row);
            }

            if (newGuests.Count > 0)
            {
                // Lưu Id lớn nhất đã xử lý
                var lastRow = newGuests.Last();
                if (lastRow.ContainsKey(ds.TriggerColumn))
                    ds.LastPolledId = Convert.ToInt64(lastRow[ds.TriggerColumn]);

                await db.SaveChangesAsync();

                foreach (var guest in newGuests)
                {
                    await _hubContext.Clients
                        .Group($"screen_{screen.Id}")
                        .SendAsync("GuestCheckedIn", guest);

                    if (newGuests.Count > 1)
                        await Task.Delay(9000);
                }

                _logger.LogInformation("Screen {Id}: {Count} record mới", screen.Id, newGuests.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Poll screen {Id} failed: {Msg}", screen.Id, ex.Message);
        }
    }
}