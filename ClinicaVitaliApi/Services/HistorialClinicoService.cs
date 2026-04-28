using ClinicaVitaliApi.DTOs;
using ClinicaVitaliApi.Models;

namespace ClinicaVitaliApi.Services;

public class HistorialClinicoService
{
    private readonly JsonFileStore<HistorialClinico> _store;
    private readonly PacienteService _pacienteService;
    private readonly MedicoService _medicoService;

    public HistorialClinicoService(IConfiguration configuration, IWebHostEnvironment environment, PacienteService pacienteService, MedicoService medicoService)
    {
        var relativePath = configuration["DataFiles:Historiales"] ?? "Data/json/historiales.json";
        var fullPath = Path.Combine(environment.ContentRootPath, relativePath);
        _store = new JsonFileStore<HistorialClinico>(fullPath);
        _pacienteService = pacienteService;
        _medicoService = medicoService;
    }

    public Task<List<HistorialClinico>> GetAllAsync() => _store.ReadAllAsync();

    public async Task<HistorialClinico?> GetByIdAsync(int id)
    {
        var items = await _store.ReadAllAsync();
        return items.FirstOrDefault(x => x.Id == id);
    }

    public async Task<(bool Success, string Message, HistorialClinico? Item)> CreateAsync(CreateHistorialClinicoDto dto)
    {
        if (await _pacienteService.GetByIdAsync(dto.IdPaciente) is null)
        {
            return (false, "El paciente indicado no existe.", null);
        }

        if (await _medicoService.GetByIdAsync(dto.IdMedico) is null)
        {
            return (false, "El medico indicado no existe.", null);
        }

        var items = await _store.ReadAllAsync();
        var historial = new HistorialClinico
        {
            Id = items.Count == 0 ? 1 : items.Max(x => x.Id) + 1,
            IdPaciente = dto.IdPaciente,
            IdMedico = dto.IdMedico,
            Diagnostico = dto.Diagnostico,
            Tratamiento = dto.Tratamiento,
            FechaConsulta = dto.FechaConsulta
        };

        items.Add(historial);
        await _store.WriteAllAsync(items);
        return (true, "Historial clinico creado correctamente.", historial);
    }

    public async Task<(bool Success, string Message, HistorialClinico? Item)> UpdateAsync(int id, UpdateHistorialClinicoDto dto)
    {
        var items = await _store.ReadAllAsync();
        var historial = items.FirstOrDefault(x => x.Id == id);
        if (historial is null)
        {
            return (false, "Historial clinico no encontrado.", null);
        }

        if (dto.IdPaciente.HasValue && await _pacienteService.GetByIdAsync(dto.IdPaciente.Value) is null)
        {
            return (false, "El nuevo paciente no existe.", null);
        }

        if (dto.IdMedico.HasValue && await _medicoService.GetByIdAsync(dto.IdMedico.Value) is null)
        {
            return (false, "El nuevo medico no existe.", null);
        }

        if (dto.IdPaciente.HasValue) historial.IdPaciente = dto.IdPaciente.Value;
        if (dto.IdMedico.HasValue) historial.IdMedico = dto.IdMedico.Value;
        if (dto.Diagnostico is not null) historial.Diagnostico = dto.Diagnostico;
        if (dto.Tratamiento is not null) historial.Tratamiento = dto.Tratamiento;
        if (dto.FechaConsulta.HasValue) historial.FechaConsulta = dto.FechaConsulta.Value;

        await _store.WriteAllAsync(items);
        return (true, "Historial clinico actualizado correctamente.", historial);
    }
}
