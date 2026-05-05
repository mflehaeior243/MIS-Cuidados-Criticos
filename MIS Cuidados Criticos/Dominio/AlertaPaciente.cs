using System.Text.Json.Serialization;
namespace MIS_Cuidados_Criticos.Dominio
{
    public class AlertaPaciente
    {
        public int Id { get; set; }
        public int Id_alerta { get; set; }
        public int Id_Paciente {  get; set; }
        [JsonIgnore]
        public Alerta alerta { get; set; }
        [JsonIgnore]
        public Paciente paciente { get; set; }
    }
}
