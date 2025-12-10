namespace InvoiceExtractor.Api.Configuration;

public class AiSettings
{
    public string OllamaUrl { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 60;
    public string SystemPrompt { get; set; } = string.Empty;
}