namespace ClinicaVitaliApi.DTOs;

public class CreateCitaMedicaDto
{
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public DateTime Fecha { get; set; }
    public string Hora { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}
