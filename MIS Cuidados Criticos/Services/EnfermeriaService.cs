using System.Text.Json;

public class EnfermeriaService
{
    private readonly HttpClient _http;

    public EnfermeriaService(HttpClient http)
    {
        _http = http;
    }

    public async Task<object> ObtenerEnfermerasDisponibles()
    {
        var response = await _http.GetAsync("https://gestionenfermeria-be-production.up.railway.app/api/Consultas/EnfermerasDisponibles");

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<object>(json);
    }
}