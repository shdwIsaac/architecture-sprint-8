using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace sprint8;

[ApiController]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "prothetic_user")]
    public IActionResult GetReport()
    {
        // Генерация фейковых данных отчёта
        var report = new
        {
            UserId = User.Identity?.Name,
            ReportDate = DateTime.UtcNow,
            Data = "Sample data about prosthetics usage"
        };
        return Ok(report);
    }
}