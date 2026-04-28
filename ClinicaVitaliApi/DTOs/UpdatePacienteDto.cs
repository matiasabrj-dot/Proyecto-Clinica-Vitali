namespace ClinicaVitaliApi.DTOs;

public class UpdatePacienteDto
{
    public string? Nombre { get; set; }
    public string? Cedula { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? Genero { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public string? EstadoClinico { get; set; }
    public DateTime? FechaRegistro { get; set; }
}
