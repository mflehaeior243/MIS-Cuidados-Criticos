using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MIS_Cuidados_Criticos.Dominio
{
    public class Alerta
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        public string? Codigo { get; set; }

        [JsonIgnore]
        public string? Estado { get; set; }

        public string Tipo { get; set; }
        public string Nivel_criticidad { get; set; }

        // Relaciones
        [JsonIgnore]
        public ICollection<SignoAlerta>? SignoAlertas { get; set; }

        [JsonIgnore]
        public ICollection<AlertaPaciente>? AlertaPacientes { get; set; }
    }
}