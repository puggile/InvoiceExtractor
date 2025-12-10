using InvoiceExtractor.Api.Configuration;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace InvoiceExtractor.Api.Services;

public interface IAiExtractorService
{
    Task<string> ExtractDataAsync(Stream imageStream, string prompt);
}

public class OllamaService : IAiExtractorService
{
    private readonly HttpClient _httpClient;
    private readonly AiSettings _settings;

    public OllamaService(HttpClient httpClient, IOptions<AiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;

        // Setup HttpClient
        _httpClient.BaseAddress = new Uri(_settings.OllamaUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
    }

    public async Task<string> ExtractDataAsync(Stream imageStream, string prompt)
    {
        // Convert the image stream to Base64
        string base64Image;
        using (var memoryStream = new MemoryStream())
        {
            await imageStream.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();
            base64Image = Convert.ToBase64String(imageBytes);
        }

        // Prepare Ollama request
        var requestPayload = new OllamaGenerateRequest
        {
            Model = _settings.ModelName,
            Prompt = prompt,
            Images = new List<string> { base64Image },
            Stream = false,
            Format = "json",
            Think = true
        };

        var jsonPayload = JsonSerializer.Serialize(requestPayload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/generate", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Errore Ollama: {response.StatusCode}");
        }

        var responseString = await response.Content.ReadAsStringAsync();

        // Debug
        Console.WriteLine(responseString);

        var ollamaResponse = JsonSerializer.Deserialize<OllamaGenerateResponse>(responseString);

        if (ollamaResponse == null) return "{}";

        // FALLBACK LOGIC:
        // If there is text in Thinking, we use that (typical of Qwen/DeepSeek).
        // Otherwise, we use Response.
        var rawContent = !string.IsNullOrWhiteSpace(ollamaResponse.Thinking)
            ? ollamaResponse.Thinking
            : ollamaResponse.Response;

        return CleanJson(rawContent);
    }

    // Clean unwanted markdown makers
    private static string CleanJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "{}";

        // Remove ```json
        var cleaned = input.Replace("```json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "", StringComparison.OrdinalIgnoreCase);

        return cleaned.Trim();
    }
}