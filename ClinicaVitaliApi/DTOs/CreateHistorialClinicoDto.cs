namespace ClinicaVitaliApi.DTOs;

public class CreateHistorialClinicoDto
{
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public string Diagnostico { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = string.Empty;
    public DateTime FechaConsulta { get; set; }
}
