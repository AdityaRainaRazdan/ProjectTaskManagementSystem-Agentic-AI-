using System.Text;
using System.Text.Json;
using DevExpress.Data.Filtering;
public class LangChainService
{
    private readonly HttpClient _http;

    public LangChainService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CrudCommand?> ParseAsync(string message)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(new { message }),
            Encoding.UTF8,
            "application/json");

        var response = await _http.PostAsync("http://127.0.0.1:8000/parse", content);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<CrudCommand>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}

public class CrudCommand
{
    public string Entity { get; set; }
    public string Action { get; set; }
    public Dictionary<string, object> Fields { get; set; }
    public Dictionary<string, object> Filters { get; set; } 
    public string LogicalOperator { get; set; } = "AND"; // AND / OR

}
