using System.Text.Json.Serialization;
namespace MIS_Cuidados_Criticos.Dominio
{
    public class SignoAlerta
    {
        public int Id { get; set; }

        public int Id_signo_vital { get; set; }
        [JsonIgnore]
        public SignoVital SignoVital { get; set; }

        public int Id_alerta { get; set; }
        [JsonIgnore]
        public Alerta Alerta { get; set; }
    }
}
