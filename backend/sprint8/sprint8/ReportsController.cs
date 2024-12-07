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
            UserId = User.Claims.FirstOrDefault(e=>e.Type=="name")?.Value,
            ReportDate = DateTime.UtcNow,
            Data = "Sample data about prosthetics usage"
        };
        return Ok(report);
    }
    
    [Authorize]
    [HttpGet("post")]
    public IActionResult DebugRoles()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        Console.WriteLine("User Claims:");
        foreach (var claim in claims)
        {
            Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
        }
        return Ok(claims);
    }
}
