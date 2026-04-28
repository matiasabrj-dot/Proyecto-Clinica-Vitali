using ClinicaVitaliApi.DTOs;
using ClinicaVitaliApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaVitaliApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "BasicAuthentication")]
public class PacientesController : ControllerBase
{
    private readonly PacienteService _service;

    public PacientesController(PacienteService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound("Paciente no encontrado.") : Ok(item);
    }

    [HttpGet("cedula/{cedula}")]
    public async Task<IActionResult> GetByCedula(string cedula)
    {
        var item = await _service.GetByCedulaAsync(cedula);
        return item is null ? NotFound("Paciente no encontrado.") : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePacienteDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Item!.Id }, result.Item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePacienteDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (!result.Success)
        {
            return result.Message.Contains("no encontrado", StringComparison.OrdinalIgnoreCase)
                ? NotFound(result.Message)
                : BadRequest(result.Message);
        }

        return Ok(result.Item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? Ok("Paciente eliminado correctamente.") : NotFound("Paciente no encontrado.");
    }
}
