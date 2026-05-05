using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace MIS_Cuidados_Criticos.Dominio
{
    public class Paciente
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        public string Codigo { get; set; }
        [JsonIgnore]
        public string? Estado { get; set; }
        public string Nomre { get; set; }   
        [JsonIgnore]
        public ICollection<SignoPaciente>? SignoPacientes{ get; set; }
        [JsonIgnore]
        public ICollection<AlertaPaciente>? AlertaPacientes { get; set; }

    }
}
