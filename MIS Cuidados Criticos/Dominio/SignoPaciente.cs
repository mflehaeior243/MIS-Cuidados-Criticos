using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace MIS_Cuidados_Criticos.Dominio
{
    public class SignoPaciente
    {
        [Key]
        public int Id { get; set; }
        public int Id_paciente { get; set; }
        public int id_signo { get; set; }
        public DateTime Fecha_hora { get; set; }
        [JsonIgnore]
        public Paciente paciente { get; set; }
        [JsonIgnore]
        public SignoVital signoVital { get; set; }
    }
}
