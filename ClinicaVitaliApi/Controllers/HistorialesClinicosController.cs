using ClinicaVitaliApi.DTOs;
using ClinicaVitaliApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaVitaliApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "BasicAuthentication")]
public class HistorialesClinicosController : ControllerBase
{
    private readonly HistorialClinicoService _service;

    public HistorialesClinicosController(HistorialClinicoService service)
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
        return item is null ? NotFound("Historial clinico no encontrado.") : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateHistorialClinicoDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Item!.Id }, result.Item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateHistorialClinicoDto dto)
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
}
