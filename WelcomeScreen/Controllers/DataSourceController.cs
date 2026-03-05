using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WelcomeScreen.API.Services;

namespace WelcomeScreen.API.Controllers;

[ApiController]
[Route("api/welcomescreens/{screenId}/datasource")]
[Authorize]
public class DataSourceController(DataSourceService service) : ControllerBase
{
    [HttpGet("test")]
    public async Task<IActionResult> GetTables([FromQuery] string connectionString)
    {
        try
        {
            var tables = await service.GetTables(connectionString);
            return Ok(tables);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("columns")]
    public async Task<IActionResult> GetColumns([FromQuery] string connectionString, [FromQuery] string tableName)
    {
        try
        {
            var columns = await service.GetColumns(connectionString, tableName);
            return Ok(columns);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save(int screenId, [FromBody] SaveDataSourceDto dto)
    {
        var ds = await service.Save(screenId, dto);
        return Ok(ds);
    }
}