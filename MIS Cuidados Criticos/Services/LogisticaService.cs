using System.Text;
using System.Text.Json;

public class LogisticaService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public LogisticaService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _baseUrl = config["Services:Logistica"];
    }

    public async Task<object> SolicitarEspacioPaciente(object data)
    {
        var json = JsonSerializer.Serialize(data);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync($"{_baseUrl}/api/Ingresos", content);

        if (!response.IsSuccessStatusCode)
        {
            return new
            {
                error = "No se pudo asignar espacio",
                status = response.StatusCode
            };
        }

        var result = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<object>(result);
    }
}