using ClinicaVitaliApi.DTOs;
using ClinicaVitaliApi.Models;

namespace ClinicaVitaliApi.Services;

public class PacienteService
{
    private readonly JsonFileStore<Paciente> _store;

    public PacienteService(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var relativePath = configuration["DataFiles:Pacientes"] ?? "Data/json/pacientes.json";
        var fullPath = Path.Combine(environment.ContentRootPath, relativePath);
        _store = new JsonFileStore<Paciente>(fullPath);
    }

    public Task<List<Paciente>> GetAllAsync() => _store.ReadAllAsync();

    public async Task<Paciente?> GetByIdAsync(int id)
    {
        var items = await _store.ReadAllAsync();
        return items.FirstOrDefault(x => x.Id == id);
    }

    public async Task<Paciente?> GetByCedulaAsync(string cedula)
    {
        var items = await _store.ReadAllAsync();
        return items.FirstOrDefault(x => x.Cedula.Equals(cedula, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<(bool Success, string Message, Paciente? Item)> CreateAsync(CreatePacienteDto dto)
    {
        var items = await _store.ReadAllAsync();
        if (items.Any(x => x.Cedula.Equals(dto.Cedula, StringComparison.OrdinalIgnoreCase)))
        {
            return (false, "Ya existe un paciente con esa cedula.", null);
        }

        var paciente = new Paciente
        {
            Id = items.Count == 0 ? 1 : items.Max(x => x.Id) + 1,
            Nombre = dto.Nombre,
            Cedula = dto.Cedula,
            FechaNacimiento = dto.FechaNacimiento,
            Genero = dto.Genero,
            Direccion = dto.Direccion,
            Telefono = dto.Telefono,
            Correo = dto.Correo,
            EstadoClinico = dto.EstadoClinico,
            FechaRegistro = DateTime.Now
        };

        items.Add(paciente);
        await _store.WriteAllAsync(items);
        return (true, "Paciente creado correctamente.", paciente);
    }

    public async Task<(bool Success, string Message, Paciente? Item)> UpdateAsync(int id, UpdatePacienteDto dto)
    {
        var items = await _store.ReadAllAsync();
        var paciente = items.FirstOrDefault(x => x.Id == id);
        if (paciente is null)
        {
            return (false, "Paciente no encontrado.", null);
        }

        if (dto.Cedula is not null && items.Any(x => x.Id != id && x.Cedula.Equals(dto.Cedula, StringComparison.OrdinalIgnoreCase)))
        {
            return (false, "La cedula ya pertenece a otro paciente.", null);
        }

        if (dto.Nombre is not null) paciente.Nombre = dto.Nombre;
        if (dto.Cedula is not null) paciente.Cedula = dto.Cedula;
        if (dto.FechaNacimiento.HasValue) paciente.FechaNacimiento = dto.FechaNacimiento.Value;
        if (dto.Genero is not null) paciente.Genero = dto.Genero;
        if (dto.Direccion is not null) paciente.Direccion = dto.Direccion;
        if (dto.Telefono is not null) paciente.Telefono = dto.Telefono;
        if (dto.Correo is not null) paciente.Correo = dto.Correo;
        if (dto.EstadoClinico is not null) paciente.EstadoClinico = dto.EstadoClinico;
        if (dto.FechaRegistro.HasValue) paciente.FechaRegistro = dto.FechaRegistro.Value;

        await _store.WriteAllAsync(items);
        return (true, "Paciente actualizado correctamente.", paciente);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var items = await _store.ReadAllAsync();
        var paciente = items.FirstOrDefault(x => x.Id == id);
        if (paciente is null)
        {
            return false;
        }

        items.Remove(paciente);
        await _store.WriteAllAsync(items);
        return true;
    }
}
