using ClinicaVitaliApi.DTOs;
using ClinicaVitaliApi.Models;

namespace ClinicaVitaliApi.Services;

public class CitaMedicaService
{
    private readonly JsonFileStore<CitaMedica> _store;
    private readonly PacienteService _pacienteService;
    private readonly MedicoService _medicoService;

    public CitaMedicaService(IConfiguration configuration, IWebHostEnvironment environment, PacienteService pacienteService, MedicoService medicoService)
    {
        var relativePath = configuration["DataFiles:Citas"] ?? "Data/json/citas.json";
        var fullPath = Path.Combine(environment.ContentRootPath, relativePath);
        _store = new JsonFileStore<CitaMedica>(fullPath);
        _pacienteService = pacienteService;
        _medicoService = medicoService;
    }

    public Task<List<CitaMedica>> GetAllAsync() => _store.ReadAllAsync();

    public async Task<CitaMedica?> GetByIdAsync(int id)
    {
        var items = await _store.ReadAllAsync();
        return items.FirstOrDefault(x => x.Id == id);
    }

    public async Task<(bool Success, string Message, CitaMedica? Item)> CreateAsync(CreateCitaMedicaDto dto)
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
        var cita = new CitaMedica
        {
            Id = items.Count == 0 ? 1 : items.Max(x => x.Id) + 1,
            IdPaciente = dto.IdPaciente,
            IdMedico = dto.IdMedico,
            Fecha = dto.Fecha,
            Hora = dto.Hora,
            Especialidad = dto.Especialidad,
            Estado = dto.Estado
        };

        items.Add(cita);
        await _store.WriteAllAsync(items);
        return (true, "Cita creada correctamente.", cita);
    }

    public async Task<(bool Success, string Message, CitaMedica? Item)> UpdateAsync(int id, UpdateCitaMedicaDto dto)
    {
        var items = await _store.ReadAllAsync();
        var cita = items.FirstOrDefault(x => x.Id == id);
        if (cita is null)
        {
            return (false, "Cita no encontrada.", null);
        }

        if (dto.IdPaciente.HasValue && await _pacienteService.GetByIdAsync(dto.IdPaciente.Value) is null)
        {
            return (false, "El nuevo paciente no existe.", null);
        }

        if (dto.IdMedico.HasValue && await _medicoService.GetByIdAsync(dto.IdMedico.Value) is null)
        {
            return (false, "El nuevo medico no existe.", null);
        }

        if (dto.IdPaciente.HasValue) cita.IdPaciente = dto.IdPaciente.Value;
        if (dto.IdMedico.HasValue) cita.IdMedico = dto.IdMedico.Value;
        if (dto.Fecha.HasValue) cita.Fecha = dto.Fecha.Value;
        if (dto.Hora is not null) cita.Hora = dto.Hora;
        if (dto.Especialidad is not null) cita.Especialidad = dto.Especialidad;
        if (dto.Estado is not null) cita.Estado = dto.Estado;

        await _store.WriteAllAsync(items);
        return (true, "Cita actualizada correctamente.", cita);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var items = await _store.ReadAllAsync();
        var cita = items.FirstOrDefault(x => x.Id == id);
        if (cita is null)
        {
            return false;
        }

        items.Remove(cita);
        await _store.WriteAllAsync(items);
        return true;
    }
}
