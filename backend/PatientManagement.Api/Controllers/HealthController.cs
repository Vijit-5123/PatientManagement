using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Infrastructure.Persistence;

namespace PatientManagement.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HealthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult Get() => Ok(new { status = "healthy" });

    [HttpGet("ready")]
    public async Task<ActionResult> Ready(CancellationToken cancellationToken)
    {
        var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
        if (!canConnect)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { status = "unready", database = "unreachable" });
        }

        return Ok(new { status = "ready", database = "connected" });
    }
}
