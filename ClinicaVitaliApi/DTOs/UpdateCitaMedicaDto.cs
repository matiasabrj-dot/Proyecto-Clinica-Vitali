namespace ClinicaVitaliApi.DTOs;

public class UpdateCitaMedicaDto
{
    public int? IdPaciente { get; set; }
    public int? IdMedico { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Hora { get; set; }
    public string? Especialidad { get; set; }
    public string? Estado { get; set; }
}
