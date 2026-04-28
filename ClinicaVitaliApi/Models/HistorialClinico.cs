namespace ClinicaVitaliApi.Models;

public class HistorialClinico : IEntity
{
    public int Id { get; set; }
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public string Diagnostico { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = string.Empty;
    public DateTime FechaConsulta { get; set; }
}
