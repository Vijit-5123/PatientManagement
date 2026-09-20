using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Api.Authorization;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Interfaces;

namespace PatientManagement.Api.Controllers;

[ApiController]
[Route("api/patients")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    /// <summary>
    /// Lists patients with optional search query and pagination. Requires Admin or Doctor role.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = PolicyNames.PatientRead)]
    [ProducesResponseType(typeof(PagedResult<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PatientDto>>> Search(
        [FromQuery] string? query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _patientService.SearchAsync(query, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a single patient by id. Requires Admin or Doctor role.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PolicyNames.PatientRead)]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _patientService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new patient. Requires Admin or Doctor role.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = PolicyNames.PatientWrite)]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientRequest request, CancellationToken cancellationToken)
    {
        var result = await _patientService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing patient. Requires Admin or Doctor role.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = PolicyNames.PatientWrite)]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> Update(int id, [FromBody] UpdatePatientRequest request, CancellationToken cancellationToken)
    {
        var result = await _patientService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a patient. Requires Admin role.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = PolicyNames.PatientDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _patientService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
