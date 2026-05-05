using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MIS_Cuidados_Criticos.Dominio
{
    public class SignoVital
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        public string? Codigo { get; set; }

        [JsonIgnore]
        public string? Estado { get; set; }

        public int Frecuencia_cardiaca { get; set; }
        public string Presion_arterial { get; set; }
        public float Saturacion_oxigeno { get; set; }

        // Relaciones
        [JsonIgnore]
        public ICollection<SignoAlerta>? signoAlertas { get; set; }

        [JsonIgnore]
        public ICollection<SignoPaciente>? signopacientes{ get; set; }
    }
}