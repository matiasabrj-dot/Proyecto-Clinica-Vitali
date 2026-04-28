using ClinicaVitaliApi.DTOs;
using ClinicaVitaliApi.Models;

namespace ClinicaVitaliApi.Services;

public class MedicoService
{
    private readonly JsonFileStore<Medico> _store;

    public MedicoService(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var relativePath = configuration["DataFiles:Medicos"] ?? "Data/json/medicos.json";
        var fullPath = Path.Combine(environment.ContentRootPath, relativePath);
        _store = new JsonFileStore<Medico>(fullPath);
    }

    public Task<List<Medico>> GetAllAsync() => _store.ReadAllAsync();

    public async Task<Medico?> GetByIdAsync(int id)
    {
        var items = await _store.ReadAllAsync();
        return items.FirstOrDefault(x => x.Id == id);
    }

    public async Task<(bool Success, string Message, Medico? Item)> CreateAsync(CreateMedicoDto dto)
    {
        var items = await _store.ReadAllAsync();
        if (items.Any(x => x.CedulaProfesional.Equals(dto.CedulaProfesional, StringComparison.OrdinalIgnoreCase)))
        {
            return (false, "Ya existe un medico con esa cedula profesional.", null);
        }

        var medico = new Medico
        {
            Id = items.Count == 0 ? 1 : items.Max(x => x.Id) + 1,
            Nombre = dto.Nombre,
            CedulaProfesional = dto.CedulaProfesional,
            Especialidad = dto.Especialidad,
            Telefono = dto.Telefono,
            Correo = dto.Correo,
            HorarioConsulta = dto.HorarioConsulta,
            Estado = dto.Estado
        };

        items.Add(medico);
        await _store.WriteAllAsync(items);
        return (true, "Medico creado correctamente.", medico);
    }

    public async Task<(bool Success, string Message, Medico? Item)> UpdateAsync(int id, UpdateMedicoDto dto)
    {
        var items = await _store.ReadAllAsync();
        var medico = items.FirstOrDefault(x => x.Id == id);
        if (medico is null)
        {
            return (false, "Medico no encontrado.", null);
        }

        if (dto.CedulaProfesional is not null && items.Any(x => x.Id != id && x.CedulaProfesional.Equals(dto.CedulaProfesional, StringComparison.OrdinalIgnoreCase)))
        {
            return (false, "La cedula profesional ya pertenece a otro medico.", null);
        }

        if (dto.Nombre is not null) medico.Nombre = dto.Nombre;
        if (dto.CedulaProfesional is not null) medico.CedulaProfesional = dto.CedulaProfesional;
        if (dto.Especialidad is not null) medico.Especialidad = dto.Especialidad;
        if (dto.Telefono is not null) medico.Telefono = dto.Telefono;
        if (dto.Correo is not null) medico.Correo = dto.Correo;
        if (dto.HorarioConsulta is not null) medico.HorarioConsulta = dto.HorarioConsulta;
        if (dto.Estado is not null) medico.Estado = dto.Estado;

        await _store.WriteAllAsync(items);
        return (true, "Medico actualizado correctamente.", medico);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var items = await _store.ReadAllAsync();
        var medico = items.FirstOrDefault(x => x.Id == id);
        if (medico is null)
        {
            return false;
        }

        items.Remove(medico);
        await _store.WriteAllAsync(items);
        return true;
    }
}
