using System.Text.Json.Serialization;

namespace MIS_Cuidados_Criticos.DTO_s
{
    public class EjemploDTO
    {
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }
    }
}
