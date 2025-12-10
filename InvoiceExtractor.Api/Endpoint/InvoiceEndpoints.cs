using InvoiceExtractor.Api.Configuration;
using InvoiceExtractor.Api.Database;
using InvoiceExtractor.Api.Services;
using Microsoft.Extensions.Options;

namespace InvoiceExtractor.Api.Endpoint;

public static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/invoices")
                       .WithTags("Invoices")
                       .WithOpenApi()
                       .DisableAntiforgery();

        group.MapPost("/analyze", AnalyzeInvoice);
    }

    private static async Task<IResult> AnalyzeInvoice(
        IFormFile file,
        IAiExtractorService aiService,
        ApplicationDbContext db,
        IInvoiceParser parser, 
        IOptions<AiSettings> settings
        )
    {
        if (file.Length == 0)
            return Results.BadRequest("Nessun file caricato");

        // Basic extension validation
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            return Results.BadRequest("Formato non supportato. Usa JPG o PNG.");

        string jsonResult;
        try
        {
            await using var stream = file.OpenReadStream();
            jsonResult = await aiService.ExtractDataAsync(stream, settings.Value.SystemPrompt);
        }
        catch (Exception ex)
        {
            // TODO logger
            return Results.Problem($"Errore durante l'analisi AI: {ex.Message}");
        }

        // Entity creation
        var record = new InvoiceRecord
        {
            FileName = file.FileName,
            RawJson = jsonResult,
            CreatedAt = DateTime.UtcNow
        };

        // Parsing Logic
        parser.MapJsonToEntity(jsonResult, record);

        db.Invoices.Add(record);
        await db.SaveChangesAsync();

        return Results.Ok(record);
    }
}