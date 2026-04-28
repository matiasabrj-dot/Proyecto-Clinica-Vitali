namespace ClinicaVitaliApi.DTOs;

public class UpdateHistorialClinicoDto
{
    public int? IdPaciente { get; set; }
    public int? IdMedico { get; set; }
    public string? Diagnostico { get; set; }
    public string? Tratamiento { get; set; }
    public DateTime? FechaConsulta { get; set; }
}
