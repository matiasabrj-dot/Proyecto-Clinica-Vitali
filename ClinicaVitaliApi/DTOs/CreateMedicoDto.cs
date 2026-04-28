namespace ClinicaVitaliApi.DTOs;

public class CreateMedicoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string CedulaProfesional { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string HorarioConsulta { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}
