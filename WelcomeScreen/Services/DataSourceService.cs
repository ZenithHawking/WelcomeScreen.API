using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WelcomeScreen.API.Data;
using WelcomeScreen.API.Models;

namespace WelcomeScreen.API.Services;

public class DataSourceService(AppDbContext db)
{
    // Test kết nối và trả về danh sách cột của table
    public async Task<List<string>> GetColumns(string connectionString, string tableName)
    {
        var columns = new List<string>();
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();
        var cmd = new SqlCommand(
            $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table ORDER BY ORDINAL_POSITION",
            conn);
        cmd.Parameters.AddWithValue("@table", tableName);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            columns.Add(reader.GetString(0));
        return columns;
    }

    // Lấy danh sách table trong database
    public async Task<List<string>> GetTables(string connectionString)
    {
        var tables = new List<string>();
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();
        var cmd = new SqlCommand(
            "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            tables.Add(reader.GetString(0));
        return tables;
    }

    public async Task<DataSource> Save(int welcomeScreenId, SaveDataSourceDto dto)
    {
        var existing = await db.DataSources.FirstOrDefaultAsync(d => d.WelcomeScreenId == welcomeScreenId);
        if (existing != null)
        {
            existing.DbType = dto.DbType;
            existing.ConnectionString = dto.ConnectionString;
            existing.TableName = dto.TableName;
            existing.TriggerColumn = dto.TriggerColumn;
            await db.SaveChangesAsync();
            return existing;
        }
        var ds = new DataSource
        {
            WelcomeScreenId = welcomeScreenId,
            DbType = dto.DbType,
            ConnectionString = dto.ConnectionString,
            TableName = dto.TableName,
            TriggerColumn = dto.TriggerColumn
        };
        db.DataSources.Add(ds);
        await db.SaveChangesAsync();
        return ds;
    }

    // Lấy khách mới check-in (polling)
    public async Task<List<Dictionary<string, object>>> GetNewGuests(DataSource ds, List<string> columns)
    {
        var result = new List<Dictionary<string, object>>();
        var colList = string.Join(", ", columns.Select(c => $"[{c}]"));
        await using var conn = new SqlConnection(ds.ConnectionString);
        await conn.OpenAsync();
        var sql = $"SELECT {colList} FROM [{ds.TableName}] WHERE [{ds.TriggerColumn}] > @lastPoll ORDER BY [{ds.TriggerColumn}]";
        var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@lastPoll", ds.LastPolledId);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
                row[reader.GetName(i)] = reader.GetValue(i);
            result.Add(row);
        }
        return result;
    }
}

public record SaveDataSourceDto(string DbType, string ConnectionString, string TableName, string TriggerColumn);