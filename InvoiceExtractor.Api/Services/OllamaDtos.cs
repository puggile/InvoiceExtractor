using System.Text.Json.Serialization;

namespace InvoiceExtractor.Api.Services;

// Request to Ollama
public class OllamaGenerateRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("images")]
    public List<string> Images { get; set; } = new();

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;

    [JsonPropertyName("format")]
    public string Format { get; set; } = "json";

    [JsonPropertyName("think")]
    public bool Think { get; set; } = false;
}

// Response from Ollama
public class OllamaGenerateResponse
{
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;

    [JsonPropertyName("thinking")]
    public string Thinking { get; set; } = string.Empty;

    [JsonPropertyName("done")]
    public bool Done { get; set; }
}